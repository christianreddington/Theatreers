using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Theatreers.Core.OtherProviders
{
  public class SHA512HashProvider
  {
    public string GetHash(object input)
    {

      // Create a new Stringbuilder to collect the bytes
      // and create a string.
      StringBuilder sBuilder = new StringBuilder();

      using (SHA512 Hash = SHA512.Create())
      {

        // Convert the input string to a byte array and compute the hash.
        byte[] dataout = Hash.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(input, Formatting.None)));

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < dataout.Length; i++)
        {
          sBuilder.Append(dataout[i].ToString("x2"));
        }
      }

      // Return the hexadecimal string.
      return sBuilder.ToString();
    }

    // Verify a Hash Value 
    public bool VerifyHash(object input, string hash)
    {
      using (SHA512 md5Hash = SHA512.Create())
      {
        // Hash the input.
        string hashOfInput = GetHash(input);

        // Create a StringComparer an compare the hashes.
        StringComparer comparer = StringComparer.Ordinal;

        if (0 == comparer.Compare(hashOfInput, hash))
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    // HashMatch2 Objects
    public bool VerifyHashMatch2Objects(object input1, object input2)
    {
      using (SHA512 md5Hash = SHA512.Create())
      {
        // Hash the input.
        string hashOfInput1 = GetHash(input1);
        string hashOfInput2 = GetHash(input2);

        // Create a StringComparer an compare the hashes.
        StringComparer comparer = StringComparer.Ordinal;

        if (0 == comparer.Compare(hashOfInput1, hashOfInput2))
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }
  }
}
