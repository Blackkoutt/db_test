﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.Models
{
    public class FAQ
    {
        [
            
            Display(Name = "Id pytania"),
            Range(0, 999)]
        public int FAQId { get; set; }

        [BindProperty(SupportsGet = true),
            Required,
            Display(Name = "Pytanie"),
            MaxLength(200, ErrorMessage = "Pytanie nie może zawierać więcej niż 200 znaków")]
        public string question { get; set; }

        [BindProperty(SupportsGet = true),
            Required,
            Display(Name = "Odpowiedź"),
            MaxLength(200, ErrorMessage = "Odpowiedź nie może zawierać więcej niż 200 znaków")]
        public string answer { get; set; }
    }
}
