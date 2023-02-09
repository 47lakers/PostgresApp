using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLApp.Models;
using SQLApp.Models.ViewModel;

namespace SQLApp.Controllers;

public class HomeController : Controller
{
    // Go to this link for ef
    // https://stackoverflow.com/questions/45382536/how-to-enable-migrations-in-visual-studio-for-mac

    private readonly MovieDataContext _context;

    public HomeController(MovieDataContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var movies = this._context.Movies.Include("Actors").Select(m => new MovieViewModel
        {
            Title = m.Title,
            Year = m.Year.ToString(),
            Summary = m.Summary,
            Actors = String.Join(',', m.Actors.Select(a => a.Fullname))
        });

        return View(movies);
    }

    // Create
    [HttpPost]
    public void CreateMovie(string title, int year, string summary, string actor1, string actor2, string actor3)
    {
        List<Actor> actors = new List<Actor>();
        if (actor1 != null)
        {
            Actor actor = new Actor();
            actor.Fullname = actor1;
            actors.Add(actor);
        }

        if (actor2 != null)
        {
            Actor actor = new Actor();
            actor.Fullname = actor2;
            actors.Add(actor);
        }

        if (actor3 != null)
        {
            Actor actor = new Actor();
            actor.Fullname = actor3;
            actors.Add(actor);
        }

        _context.Movies.Add(new Movie
        {
            Title = title,
            Year = year,
            Summary = summary,
            Actors = actors
        });

        _context.SaveChanges();
    }

    // Delete
    [HttpPost]
    public async Task<IActionResult> DeleteMovie(string title)
    {
        // You need the Include actors because it's an SQL foreign key thing
        // to not include any other tables that are related
        var movie = await _context.Movies.Include("Actors").Where(m => m.Title == title).FirstOrDefaultAsync();
        string message = null;
        if(movie == null)
        {
            message = "Movie does not exist!";
        }
        else
        {
            var listOfActors = movie.Actors;

            // You need to add ToList() because the listOfActors is getting modified
            // and it creates an error. When you add ToList(), nothing can modify that list
            // For more info, go here: https://stackoverflow.com/questions/604831/collection-was-modified-enumeration-operation-may-not-execute
            foreach (var actor in listOfActors.ToList())
            {
                var a = await _context.Actors.Where(a => a.Fullname == actor.Fullname).FirstOrDefaultAsync();
                if(a != null)
                {
                    _context.Actors.Remove(a);
                    _context.SaveChanges();
                }
            }

            _context.Movies.Remove(movie);
            _context.SaveChanges();
        }

        return Json(new { message });
    }

    // Edit
    [HttpPost]
    public async Task<IActionResult> EditMovie(string title, int year, string summary, string actor1, string actor2, string actor3)
    {
        // You need the Include actors because it's an SQL foreign key thing
        // to not include any other tables that are related
        var movie = await _context.Movies.Include("Actors").Where(m => m.Title == title).FirstOrDefaultAsync();
        string message = null;

        if (movie == null)
        {
            message = "Movie does not exist!";
        }
        else
        {
            var listOfActors = movie.Actors;
            // Not good practice but this is just for learning purposes
            int counter = 1;
            foreach (var actor in listOfActors.ToList())
            { 
                var a = await _context.Actors.Where(a => a.Fullname == actor.Fullname).FirstOrDefaultAsync();
                if (a != null)
                {
                    if (counter == 1)
                    {
                        a.Fullname = actor1;
                    }else if(counter == 2)
                    {
                        a.Fullname = actor2;
                    }
                    else
                    {
                        a.Fullname = actor3;
                    }
                    // Don't forget Update or it won't work
                    _context.Update(a);
                    _context.SaveChanges();
                }
                counter += 1;
            }

            movie.Title = title;
            movie.Year = year;
            movie.Summary = summary;
            _context.SaveChanges();
        }

        return Json(new { message });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

