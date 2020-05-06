using Microsoft.AspNetCore.Http;
using System;
namespace DatingAppAPI.Dtos
{
    public class PhotoForCreationDto
    {

          
                public int Id { get; set; }
          public string Url { get; set; }
           public string Description { get; set; }
             public DateTime DateAdded { get; set; }
             public bool IsMain { get; set; }

                public string PublicId { get; set; }

                public IFormFile File { get; set; }

                public PhotoForCreationDto()
                {
                    DateAdded = DateTime.Now;
                }
}
}