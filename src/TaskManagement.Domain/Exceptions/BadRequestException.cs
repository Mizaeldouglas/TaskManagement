﻿namespace TaskManagement.Domain.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}