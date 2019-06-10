namespace Console.Security
{
    public static class Encryptor
    {
        /// <summary>
        /// Encrypt bytes with a password
        /// </summary>
        /// <param name="data">Data bytes</param>
        /// <param name="password">Password bytes for encrypting</param>
        /// <returns></returns>
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