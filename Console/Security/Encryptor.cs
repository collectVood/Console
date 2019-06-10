namespace Console.Security
{
    public static class Encryptor
    {
        public static byte[] Encrypt(byte[] data, byte[] password)
        {
            if (password.Length == 0)
                return data;
            
            var encrypted = new byte[data.Length];
            var currentPasswordIndex = 0;
            for (var i = 0; i < data.Length; i++)
            {
                encrypted[i] = (byte) (data[i] ^ password[currentPasswordIndex++]);
                if (currentPasswordIndex == password.Length)
                    currentPasswordIndex = 0;
            }

            return encrypted;
        }
    }
}