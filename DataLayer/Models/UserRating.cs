using DataLayer.Models;
using System;

namespace DataLayer.Models
{
  public class UserRating
  {
    public int UserId { get; set; }
    public int TConst { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
  }
}