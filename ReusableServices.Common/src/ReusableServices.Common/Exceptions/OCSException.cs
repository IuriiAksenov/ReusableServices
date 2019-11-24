using System;

namespace ReusableServices.Common.Exceptions
{
  // ReSharper disable once InconsistentNaming
  public class OCSException : Exception
  {
    public string Code { get; protected set; }

    public OCSException()
    {
    }

    public OCSException(string code)
    {
      Code = code;
    }

    public OCSException(string message, params object[] args) : this(string.Empty, message, args)
    {
    }

    public OCSException(string code, string message, params object[] args) : this(null, code, message, args)
    {
    }

    public OCSException(Exception innerException, string message, params object[] args) : this(innerException,
      string.Empty, message, args)
    {
    }

    public OCSException(Exception innerException, string code, string message, params object[] args) : base(
      string.Format(message, args), innerException)
    {
      Code = code;
    }
  }
}