package contactdatacollection;

public class AESSymmetricEncription 
{
    String INITIALIZATION_VECTOR = "e675f725e675f725";
    
    String sPassword = "";
    String sSalt = "";
    String sInitializationVector = "";
    
    public void setPassword(String Password)
    {
        sPassword = Password;
    }
    
    public String getPassword()
    {
        return sPassword;
    }
    
    public void setSalt(String Salt)
    {
        sSalt = Salt;
    }
    
    public String getSalt()
    {
        return sSalt;
    }
    
    public void setInitializationVector(String InitializationVector)
    {
        sInitializationVector = InitializationVector;
    }
    
    public String getInitializationVector()
    {
        return sInitializationVector;
    }
    
    public AESSymmetricEncription(String Password, String Salt)
    {
        sPassword = Password;
        sSalt = Salt;
        sInitializationVector = INITIALIZATION_VECTOR;
    }
    
    public AESSymmetricEncription(String Password, String Salt, String InitializationVector)
    {
        sPassword = Password;
        sSalt = Salt;
        sInitializationVector = InitializationVector;
    }
    
    public String Encrypt(String message) throws Exception
    {
        javax.crypto.Cipher c = getCipher(javax.crypto.Cipher.ENCRYPT_MODE);
        byte[] encryptedVal = c.doFinal(message.getBytes("UTF-8"));
            
        return new sun.misc.BASE64Encoder().encode(encryptedVal);
    }
    
    public String Decrypt(String message) throws Exception
    {
        byte[] decodedValue = new sun.misc.BASE64Decoder().decodeBuffer(message);

        javax.crypto.Cipher c = getCipher(javax.crypto.Cipher.DECRYPT_MODE);
            
        byte[] decValue = c.doFinal(decodedValue);

        return new String(decValue);
    }
    
    private javax.crypto.Cipher getCipher(int mode) throws Exception
    {
        javax.crypto.Cipher c = javax.crypto.Cipher.getInstance("AES/CBC/PKCS5Padding", new com.sun.crypto.provider.SunJCE());

        byte[] iv = sInitializationVector.getBytes("UTF-8");

        c.init(mode, generateKey(), new javax.crypto.spec.IvParameterSpec(iv));
      
        return c;
    }
    
    private java.security.Key generateKey() throws Exception 
    {
      javax.crypto.SecretKeyFactory factory = javax.crypto.SecretKeyFactory.getInstance("PBKDF2WithHmacSHA1");
      char[] password = sPassword.toCharArray();
      byte[] salt = sSalt.getBytes("UTF-8");

      java.security.spec.KeySpec spec = new javax.crypto.spec.PBEKeySpec(password, salt, 65536, 128);
      javax.crypto.SecretKey tmp = factory.generateSecret(spec);
      byte[] encoded = tmp.getEncoded();
      
      return new javax.crypto.spec.SecretKeySpec(encoded, "AES");
  }
}
