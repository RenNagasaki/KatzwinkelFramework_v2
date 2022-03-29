using Framework;
using Framework.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ChatService : ISendChatService
    {
        string CryptionPassPhrase = "Katzwinkel_Framework2017@Julian-The|Master@ChatService";
        string connectionString = @"Data Source=127.0.0.1;" + "Initial Catalog=chatwinkel;" + "User id=RenDB;" + "Password=Lou1se_VallieSqL;"; //mssql
        //string connectionString = @"Data Source=DESKTOP-6RQ6Q2P\SQLEXPRESS;" + "Initial Catalog=chatwinkel;" + "User id=sa;" + "Password=Lou1se_VallieSqL;"; //mssql
        //string connectionString = "Server=localhost;" + "Database=test;" + "Uid=sa;" + "Pwd=Lou1se_VallieSA;"; //mysql
        Dictionary<string, IReceiveChatService> LoggedInUsersCallback = new Dictionary<string, IReceiveChatService>();
        Dictionary<string, ChatUser> LoggedInUsers = new Dictionary<string, ChatUser>();

        public static event ListOfNames ChatListOfNames;

        IReceiveChatService callback = null;

        public ChatService() { }

        public void Close()
        {
            callback = null;
            LoggedInUsers.Clear();
        }

        public ChatUser Login(string Name, string Password)
        {
            ChatUser user = new ChatUser();
            Name = CryptionManager.Decrypt(Name, CryptionPassPhrase);
            Password = CryptionManager.Decrypt(Password, CryptionPassPhrase);

            bool found = false;

            found = DoDatabaseLookup(ref user, Name, Password);

            if (found)
            {
                if (LoggedInUsers.ContainsKey(Name))
                {
                    Disconnect(Name);
                }
                callback = OperationContext.Current.GetCallbackChannel<IReceiveChatService>();
                AddUser(Name, callback, user);
                Console.WriteLine(String.Format("User: {0} logged in.", Name));
            }            

            return user;
        }

        public void UpdateUser(ChatUser user)
        {
            user.Desc = CryptionManager.Decrypt(user.Desc, CryptionPassPhrase);
            using (SqlConnection conn = new SqlConnection() { ConnectionString = connectionString })
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("UPDATE users SET description = @desc, image = @image WHERE Id = @id; ", conn);
                comm.Parameters.AddWithValue("@desc", user.Desc);
                comm.Parameters.AddWithValue("@image", user.profilepic);
                comm.Parameters.AddWithValue("@id", user.UserID);

                SqlManager.DoSqlCommandNonQuery(comm);

                conn.Close();
            }
        }

        private bool DoDatabaseLookup(ref ChatUser user, string Name, string Password)
        {
            bool found = false;
            using (SqlConnection conn = new SqlConnection() { ConnectionString = connectionString })
            {
                conn.Open();
                SqlDataReader reader = SqlManager.DoSqlCommandReader("SELECT * FROM users WHERE username = '" + Name + "' AND password = '"+ Password +"'", conn);
                try
                {
                    while (reader.Read())
                    {
                        found = true;
                        user.LoggedIn = true;
                        user.UserID = Convert.ToInt32(reader[0]);
                        user.UserName = CryptionManager.Encrypt(reader[1].ToString(), CryptionPassPhrase);
                        user.FullName = CryptionManager.Encrypt(reader[2].ToString(), CryptionPassPhrase);
                        user.EMail = CryptionManager.Encrypt(reader[3].ToString(), CryptionPassPhrase);
                        user.Password = "";
                        user.Desc = CryptionManager.Encrypt(reader[5].ToString(), CryptionPassPhrase);

                        byte[] bytes = reader[6] as byte[];
                        if (bytes != null && bytes.Length > 0)
                        {
                            user.profilepic = bytes;
                        }
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }

                conn.Close();
            }

            return found;
        }

        public void Disconnect(string Name)
        {
            IReceiveChatService proxy = LoggedInUsersCallback[Name];
            try
            {
                proxy.DisconnectClient();
            }
            catch { }
            RemoveUser(Name);
            Console.WriteLine(String.Format("User: {0} got kicked.", Name));
        }

        public RegisterResult Register(ChatUser user)
        {
            user.UserName = CryptionManager.Decrypt(user.UserName, CryptionPassPhrase);
            user.Password = CryptionManager.Decrypt(user.Password, CryptionPassPhrase);
            user.EMail = CryptionManager.Decrypt(user.EMail, CryptionPassPhrase);
            user.FullName = CryptionManager.Decrypt(user.FullName, CryptionPassPhrase);
            RegisterResult res = new RegisterResult();

            bool found = false;
            using (SqlConnection conn = new SqlConnection() { ConnectionString = connectionString })
            {
                conn.Open();
                SqlDataReader reader = SqlManager.DoSqlCommandReader("SELECT ID FROM users WHERE username = '" + user.UserName + "'", conn);
                try
                {
                    while (reader.Read())
                    {
                        found = true;
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }

                if (!found)
                {
                    res.Success = SqlManager.DoSqlCommandNonQuery("INSERT INTO users(username, fullname, email, password) VALUES('" + user.UserName + "', '" + user.FullName + "', '" + user.EMail + "', '" + user.Password + "')", conn);
                }
                else
                {
                    res.Errormessage = "Username already taken";
                    res.Success = false;
                }

                conn.Close();
            }

            return res;
        }

        public void Start(string Name)
        {
            try
            {
                Name = CryptionManager.Decrypt(Name, CryptionPassPhrase);
                if (LoggedInUsers.ContainsKey(Name))
                {
                    SendNamesToAll();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Stop(string Name)
        {
            try
            {
                Name = CryptionManager.Decrypt(Name, CryptionPassPhrase);
                RemoveUser(Name);
                Console.WriteLine(String.Format("User: {0} logged out.", Name));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public ChatMessage[] GetHistory(string sender, string receiver)
        {
            sender = CryptionManager.Decrypt(sender, CryptionPassPhrase);
            receiver = CryptionManager.Decrypt(receiver, CryptionPassPhrase);

            List<ChatMessage> messages = new List<ChatMessage>();

            using (SqlConnection conn = new SqlConnection() { ConnectionString = connectionString })
            {
                int senderId = -1;
                int receiverId = -1;
                conn.Open();

                SqlDataReader reader = SqlManager.DoSqlCommandReader("SELECT ID FROM users WHERE username = '" + sender + "'", conn);
                reader.Read();
                senderId = Convert.ToInt32(reader[0]);
                reader.Close();

                reader = SqlManager.DoSqlCommandReader("SELECT ID FROM users WHERE username = '" + receiver + "'", conn);
                reader.Read();
                receiverId = Convert.ToInt32(reader[0]);
                reader.Close();

                reader = SqlManager.DoSqlCommandReader("SELECT * FROM messages WHERE sender = " + senderId + " AND receiver = " + receiverId, conn);
                while (reader.Read())
                {
                    ChatMessage message = new ChatMessage(CryptionManager.Encrypt(sender, CryptionPassPhrase));
                    message.Receiver = CryptionManager.Encrypt(receiver, CryptionPassPhrase);
                    message.Message = CryptionManager.Encrypt(reader[3].ToString(), CryptionPassPhrase);
                    message.Time = reader.GetDateTime(4);

                    messages.Add(message);
                }
                reader.Close();

                reader = SqlManager.DoSqlCommandReader("SELECT * FROM messages WHERE sender = " + receiverId + " AND receiver = " + senderId, conn);
                while (reader.Read())
                {
                    ChatMessage message = new ChatMessage(CryptionManager.Encrypt(receiver, CryptionPassPhrase));
                    message.Receiver = CryptionManager.Encrypt(sender, CryptionPassPhrase);
                    message.Message = CryptionManager.Encrypt(reader[3].ToString(), CryptionPassPhrase);
                    message.Time = reader.GetDateTime(4);

                    messages.Add(message);
                }
                reader.Close();

                conn.Close();
            }

            return messages.OrderBy(o => o.Time).ToList().ToArray();
        }

        void SendNamesToAll()
        {
            foreach (KeyValuePair<string, IReceiveChatService> name in LoggedInUsersCallback)
            {
                try
                {
                    IReceiveChatService proxy = name.Value;
                    proxy.SendNames(LoggedInUsers.Values.ToList());
                }
                catch
                {
                    Disconnect(name.Key);
                }
            }

            if (ChatListOfNames != null)
                ChatListOfNames(LoggedInUsers.Values.ToList(), this);
        }

        void ISendChatService.SendMessage(ChatMessage Message)
        {
            string sender = CryptionManager.Decrypt(Message.Sender, CryptionPassPhrase);
            string receiver = CryptionManager.Decrypt(Message.Receiver, CryptionPassPhrase);
            string message = CryptionManager.Decrypt(Message.Message, CryptionPassPhrase);
            
            using (SqlConnection conn = new SqlConnection() { ConnectionString = connectionString })
            {
                int senderId = -1;
                int receiverId = -1;
                conn.Open();

                SqlDataReader reader = SqlManager.DoSqlCommandReader("SELECT ID FROM users WHERE username = '" + sender + "'", conn);
                reader.Read();
                senderId = Convert.ToInt32(reader[0]);
                reader.Close();

                reader = SqlManager.DoSqlCommandReader("SELECT ID FROM users WHERE username = '" + receiver + "'", conn);
                reader.Read();
                receiverId = Convert.ToInt32(reader[0]);
                reader.Close();

                SqlCommand comm = new SqlCommand(String.Format("INSERT INTO messages(sender, receiver, message, time) VALUES({0}, {1}, {2}, {3})", "@sender", "@receiver", "@message", "@time"), conn);
                comm.Parameters.AddWithValue("@sender", senderId);
                comm.Parameters.AddWithValue("@receiver", receiverId);
                comm.Parameters.AddWithValue("@message", message);
                comm.Parameters.AddWithValue("@time", Message.Time);

                SqlManager.DoSqlCommandNonQuery(comm);

                conn.Close();
            }

            if (LoggedInUsers.ContainsKey(receiver))
            {
                callback = LoggedInUsersCallback[receiver];
                callback.ReceiveMessage(Message);
            }
        }

        private void RemoveUser(string Name)
        {
            if (LoggedInUsers.ContainsKey(Name))
            {
                LoggedInUsers.Remove(Name);
                LoggedInUsersCallback.Remove(Name);
                SendNamesToAll();
            }
        }

        private void AddUser(string name, IReceiveChatService callback, ChatUser user)
        {
            LoggedInUsers.Add(name, user);
            LoggedInUsersCallback.Add(name, callback);
            if (ChatListOfNames != null)
            {
                ChatListOfNames(LoggedInUsers.Values.ToList(), this);
            }
        }
    }
}
