namespace net.atos.daf.ct2.portalservice.Account
{
    public class AccountBlobRequest
    {
        public int BlobId { get; set; }
        public int AccountId { get; set; }
        public string ImageType { get; set; }
        public byte[] Image { get; set; }

    }

    public class AccountBlobResponse
    {
        public int BlobId { get; set; }
        public byte[] Image { get; set; }

    }
}
