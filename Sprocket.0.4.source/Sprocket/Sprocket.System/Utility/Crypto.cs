using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Sprocket.Utility
{
	public static class Crypto
	{
		public static string EncryptOneWay(string data)
		{
			byte[] buf = Encoding.ASCII.GetBytes(data);
			SHA512 sha = new SHA512Managed();
			byte[] enc = sha.ComputeHash(buf);
			return HexEncoding.ToString(enc);
		}
	}
}
