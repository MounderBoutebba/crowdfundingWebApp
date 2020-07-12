using System;
using System.Collections.Generic;
using System.Text;

namespace Coiner.Business.CustomExceptions
{
    public class UserNotFoundException: Exception
    {
        public UserNotFoundException(int userId)
            :base($"user with userId: {userId} is not existing in database")
        {
        }
    }
}
