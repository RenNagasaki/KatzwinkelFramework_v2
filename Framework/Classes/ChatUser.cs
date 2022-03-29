using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Windows.Media.Imaging;

namespace Framework
{
    [DataContract]
    public class ChatUser
    {
        [DataMember]
        public bool LoggedIn = false;
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string EMail { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Desc { get; set; }
        [DataMember]
        public byte[] profilepic = null;

        [OperationContract]
        public BitmapImage GetProfilePic()
        {
            if (profilepic != null)
            {
                BitmapImage bmpi = new BitmapImage();
                bmpi.BeginInit();
                bmpi.StreamSource = new MemoryStream(profilepic);
                bmpi.EndInit();

                return bmpi;
            }
            else
            {
                return null;
            }
        }


        [OperationContract]
        public void SetProfilePic(BitmapImage img)
        {
            if (img != null)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                using (MemoryStream ms = new MemoryStream())
                {
                    encoder.Save(ms);
                    profilepic = ms.ToArray();
                }
            }
            else
            {
                profilepic = null;
            }
        }
    }
}
