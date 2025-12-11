using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace final_project.Models
{
    public class orders
    {

        public int Id { get; set; }
        public string custname { get; set; }
        public DateTime orderdate { get; set; }
        public int total { get; set; }
      
    }
}
