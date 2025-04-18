namespace AttendanceAPI
{
    public class Session
    {
        public User user { get; set; }
        public Guid id { get; set; }
        public DateTime expires;

        public Session(User user)
        {
            this.user = user;
            id = Guid.NewGuid();
            Renew();
        }

        public bool IsExpired()
        {
            return expires < DateTime.Now;
        }

        public void Renew()
        {
            expires = DateTime.Now + TimeSpan.FromSeconds(30);
            //expires = DateTime.Now + TimeSpan.FromHours(6);
        }
    }
}
