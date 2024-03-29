﻿namespace LibraryModels.Repository
{
    public class ErrorSerializing
    {
        public int Level { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }

        public ErrorSerializing() { }

        public ErrorSerializing((int level, string description, string message) param)
        {
            Level = param.level;
            Description = param.description;
            Message = param.message;
        }
    }
}
