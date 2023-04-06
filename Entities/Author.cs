﻿namespace Readerpath.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string Names { get; set; }
        public string Surname { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public bool isAccepted { get; set; }

        public Author()
        {
            DateAdded = DateTime.Now;
        }
    }
}
