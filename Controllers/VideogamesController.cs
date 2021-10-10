using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ApiWithoutEF.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiWithoutEF.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideogamesController : ControllerBase
    {

        Dal db = new Dal();

        //GET: api/videogames
        [HttpGet]
        public ActionResult<IEnumerable<Videogame>> GetVideogames()
        {
            DataTable dt = db.GetData("GetAllVideogames");
            List<Videogame> videogames = 
                (   
                    from row in dt.Select()
                    select new Videogame
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Title = Convert.ToString(row["Title"]),
                        Company = Convert.ToString(row["Company"]),
                        Clasification = Convert.ToString(row["Clasification"]),
                    }
                ).ToList();

                return videogames;
        }

        //GET: api/videogames/id
        [HttpGet("{id}")]
        public ActionResult<Videogame> GetVideogame(string id)
        {
            List<Parameter> parameters = new List<Parameter>()
            {
                new Parameter() { Name = "@Id", Value = id }
            };

            DataTable dt = db.GetData("GetVideogameById", parameters);

            Videogame videogame = 
            (   
                from row in dt.Select()
                select new Videogame
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Title = Convert.ToString(row["Title"]),
                    Company = Convert.ToString(row["Company"]),
                    Clasification = Convert.ToString(row["Clasification"]),
                }
            ).FirstOrDefault();

            if(videogame == null)
            {
                return NotFound();
            }

            return videogame;
        }

        //POST: api/videogames
        [HttpPost]
        public ActionResult Post(Videogame videogame)
        {
            List<Parameter> parameters = new List<Parameter>()
            {
                new Parameter() { Name = "@Title", Value = videogame.Title },
                new Parameter() { Name = "@Company", Value = videogame.Company },
                new Parameter() { Name = "@Clasification", Value = videogame.Clasification }
            };

            int id = db.SetDataWithReturn("SetVideoGame", parameters);
            videogame.Id = id;

            return CreatedAtAction(nameof(GetVideogame), new { Id = id }, videogame);      
        }

        //PUT: api/videogames/id
        [HttpPut("{id}")]
        public ActionResult<Videogame> Put(int id, Videogame videogame)
        {
            if (id != videogame.Id)
            {
                return BadRequest();
            }

            List<Parameter> parameters = new List<Parameter>()
            {
                new Parameter() { Name = "@Id", Value = videogame.Id.ToString() },
                new Parameter() { Name = "@Title", Value = videogame.Title },
                new Parameter() { Name = "@Company", Value = videogame.Company },
                new Parameter() { Name = "@Clasification", Value = videogame.Clasification }
            };

            db.SetData("UpdateVideogame", parameters);
        
            return NoContent();
        }

        //DELETE: api/videogames/id
        [HttpDelete("{id}")]
        public void DeleteCommandItem(int id)
        {   
            List<Parameter> parameters = new List<Parameter>()
            {
                new Parameter() { Name = "@Id", Value = id.ToString() }
            };
                        
            db.SetData("DeleteVideogame", parameters);
        }

    }
}
