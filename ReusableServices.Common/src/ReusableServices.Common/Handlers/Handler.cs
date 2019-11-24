using System;
using System.Threading.Tasks;
using ReusableServices.Common.Exceptions;

namespace ReusableServices.Common.Handlers
{
  public class Handler : IHandler
  {
    private Func<Task> _always;

    private Func<Task> _handle;
    private Func<OCSException, Task> _onCustomError;
    private Func<Exception, Task> _onError;
    private Func<Task> _onSuccess;
    private bool _rethrowCustomException;
    private bool _rethrowException;

    public Handler()
    {
      _always = () => Task.CompletedTask;
    }

    public IHandler Handle(Func<Task> handle)
    {
      _handle = handle;
      return this;
    }

    public IHandler OnSuccess(Func<Task> onSuccess)
    {
      _onSuccess = onSuccess;
      return this;
    }

    public IHandler OnError(Func<Exception, Task> onError, bool rethrow = false)
    {
      _onError = onError;
      _rethrowException = rethrow;
      return this;
    }

    public IHandler OnCustomError(Func<OCSException, Task> onCustomError, bool rethrow = false)
    {
      _onCustomError = onCustomError;
      _rethrowCustomException = rethrow;
      return this;
    }

    public IHandler Always(Func<Task> always)
    {
      _always = always;
      return this;
    }

    public async Task ExecuteAsync()
    {
      var isFailure = false;

      try
      {
        await _handle();
      }
      catch (OCSException customException)
      {
        isFailure = true;
        if (_onCustomError != null) await _onCustomError.Invoke(customException);
        if (_rethrowCustomException) throw;
      }
      catch (Exception exception)
      {
        isFailure = true;
        if (_onError != null) await _onError.Invoke(exception);
        if (_rethrowException) throw;
      }
      finally
      {
        if (!isFailure)
          if (_onSuccess != null)
            await _onSuccess.Invoke();

        if (_always != null) await _always.Invoke();
      }
    }
  }
}