using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataLayer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DataLayer
{
  public class DataService : IDataService
  {
    private readonly MovieDbContext _context;

    public DataService(IConfiguration configuration)
    {
      var options = new DbContextOptionsBuilder<MovieDbContext>()
          .UseNpgsql(configuration.GetConnectionString("imdbDatabase"))
          .Options;

      _context = new MovieDbContext(options);
    }

    // -- Helper Methods --

    private IList<T> GetPagedResults<T>(IQueryable<T> query, int pageNumber, int pageSize) where T : class =>
        query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

    private bool SaveChanges() => _context.SaveChanges() > 0;

    private bool Exists<T>(int id) where T : class =>
        _context.Set<T>().Find(id) != null;

    private T FindById<T>(int id) where T : class =>
        _context.Set<T>().Find(id);
        

   //Create user for authentication part
   public User CreateUser(string name, string username, string password , string email , string salt , string role = "user")
        {
            var user = new User
            {
        //     Id = _context.Users.Max(x => x.Id) + 1,
                Name = name,
                Username = username,
                Password = password,
                Email = email,
                Salt = salt,
                Role = role,
            };
            _context.Users.Add(user);
            SaveChanges();
            return user;
        }
    // -- USER --

    public User AddUser(string username, string password, string email)
    {


      var user = new User { Username = username, Password = password, Email = email };
      _context.Users.Add(user);
      SaveChanges();
      return user;
    }

    public User GetUser(string username) =>
        _context.Users.FirstOrDefault(u => u.Username == username);

    public User GetUser(int userId) =>
        FindById<User>(userId);

    public void DeleteUser(int userId)
    {
      var user = FindById<User>(userId);
      if (user != null)
      {
        _context.Users.Remove(user);
        SaveChanges();
      }
    }

    public bool UserExists(int userId) =>
        Exists<User>(userId);

    // -- BOOKMARK --

    public IList<Bookmark> GetBookmarks(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.Bookmarks.Where(b => b.UserId == userId).OrderBy(b => b.Id);
      return GetPagedResults(query, pageNumber, pageSize);
    }

    public int GetBookmarkCountByUser(int userId) =>
        _context.Bookmarks.Count(b => b.UserId == userId);

    public Bookmark GetBookmarkById(int bookmarkId) =>
        _context.Bookmarks.FirstOrDefault(b => b.Id == bookmarkId);

    public Bookmark GetBookmark(int userId, int bookmarkId) =>
        _context.Bookmarks.FirstOrDefault(b => b.UserId == userId && b.Id == bookmarkId);

    public Bookmark AddBookmark(int userId, string? tconst, string? nconst, string note)
    {
      ValidateUserExists(userId);

      if ((tconst == null && nconst == null) || (tconst != null && nconst != null))
        throw new ArgumentException("Specify either tconst or nconst, not both.");

      var bookmark = new Bookmark
      {
        UserId = userId,
        TConst = tconst,
        NConst = nconst,
        Note = note,
        CreatedAt = DateTime.UtcNow
      };
      _context.Bookmarks.Add(bookmark);
      SaveChanges();
      return bookmark;
    }

    public void UpdateBookmark(int userId, int bookmarkId, string tconst, string nconst, string note)
    {
      var bookmark = GetBookmark(userId, bookmarkId);
      if (bookmark != null)
      {
        bookmark.TConst = tconst;
        bookmark.NConst = nconst;
        bookmark.Note = note;
        bookmark.CreatedAt = DateTime.UtcNow;
        SaveChanges();
      }
    }

    public void DeleteBookmark(int bookmarkId)
    {
      var bookmark = FindById<Bookmark>(bookmarkId);
      if (bookmark != null)
      {
        _context.Bookmarks.Remove(bookmark);
        SaveChanges();
      }
    }

    // -- SEARCH HISTORY --

    public IList<SearchHistory> GetSearchHistory(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.SearchHistories.Where(sh => sh.UserId == userId).OrderBy(sh => sh.CreatedAt);
      return GetPagedResults(query, pageNumber, pageSize);
    }

    public SearchHistory GetSearchHistory(int searchId) =>
        FindById<SearchHistory>(searchId);

    public IList<SearchHistory> GetSearchHistoriesByUser(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.SearchHistories.Where(sh => sh.UserId == userId).OrderBy(sh => sh.CreatedAt);
      return GetPagedResults(query, pageNumber, pageSize);
    }

    public int GetSearchHistoryCountByUser(int userId) =>
        _context.SearchHistories.Count(sh => sh.UserId == userId);

    public SearchHistory AddSearchHistory(int userId, string searchQuery)
    {
      var searchHistory = new SearchHistory
      {
        UserId = userId,
        SearchQuery = searchQuery,
        CreatedAt = DateTime.UtcNow
      };
      _context.SearchHistories.Add(searchHistory);
      SaveChanges();
      return searchHistory;
    }

    public void DeleteSearchHistory(int searchId)
    {
      var searchHistory = FindById<SearchHistory>(searchId);
      if (searchHistory != null)
      {
        _context.SearchHistories.Remove(searchHistory);
        SaveChanges();
      }
    }

    // -- USER RATING --

    public IList<UserRating> GetUserRatings(int userId, int pageNumber = 1, int pageSize = 10)
    {
      var query = _context.UserRatings.Where(ur => ur.UserId == userId).OrderBy(ur => ur.Id);
      return GetPagedResults(query, pageNumber, pageSize);
    }

    public UserRating GetUserRating(int ratingId) =>
        FindById<UserRating>(ratingId);

    public UserRating GetUserRatingByUserAndTConst(int userId, string tconst) =>
        _context.UserRatings.FirstOrDefault(ur => ur.UserId == userId && ur.TConst == tconst);

    public int GetUserRatingCount(int userId) =>
        _context.UserRatings.Count(ur => ur.UserId == userId);

    public UserRating AddUserRating(int userId, string tconst, int rating)
    {
      var userRating = new UserRating
      {
        UserId = userId,
        TConst = tconst,
        Rating = rating,
        CreatedAt = DateTime.UtcNow
      };
      _context.UserRatings.Add(userRating);
      SaveChanges();
      return userRating;
    }

    public void UpdateUserRating(int ratingId, int rating)
    {
      var userRating = FindById<UserRating>(ratingId);
      if (userRating != null)
      {
        userRating.Rating = rating;
        userRating.CreatedAt = DateTime.UtcNow;
        SaveChanges();
      }
    }

    public void UpdateUserRating(int userId, int ratingId, int rating)
    {
      var userRating = GetUserRating(ratingId);
      if (userRating != null)
      {
        userRating.Rating = rating;
        userRating.CreatedAt = DateTime.UtcNow;
        SaveChanges();
      }
    }

    public void DeleteUserRating(int ratingId)
    {
      var userRating = FindById<UserRating>(ratingId);
      if (userRating != null)
      {
        _context.UserRatings.Remove(userRating);
        SaveChanges();
      }
    }

    // -- Private Helper Methods --

    private void ValidateUserExists(int userId)
    {
      if (!Exists<User>(userId))
        throw new ArgumentException("User with specified ID does not exist.");
    }
    // title 
    public TitleBasic GetTitleByTConst(string tConst)
    {
      return _context.TitleBasics.FirstOrDefault(tb => tb.TConst == tConst);
    }
    // coplayers
    public IList<CoPlayer> GetCoPlayers(string nConst)
    {
      return _context.CoPlayers.FromSqlInterpolated($"select * from coplayers({nConst})").ToList();
    }

    public IList<RatingActor> GetRatingActors(string tConst)
    {
      return _context.RatingActors.FromSqlInterpolated($"select * from ratingactors({tConst})").ToList();
    }
    public IList<RatingCoPlayer> GetRatingCoPlayers(string nConst)
    {
      return _context.RatingCoPlayers.FromSqlInterpolated($"select * from ratingcoplayers({nConst})").ToList();
    }
    public IList<RatingCrew> GetRatingCrew(string tConst)
    {
      return _context._RatingCrew.FromSqlInterpolated($"select * from ratingcrew({tConst})").ToList();
    }
    public IList<SimilarMovie> GetSimilarMovies(string tConst)
    {
      return _context.SimilarMovies.FromSqlInterpolated($"select * from similarmovies({tConst})").ToList();
    }
        public IList<SearchName> GetSearchNames(string searchTerm)
        {
            return _context.SearchNames.FromSqlInterpolated($"select * from search_names_by_text({searchTerm})").ToList();
        }
        public IList<SearchTitle> GetSearchTitles(string searchTerm)
        {
            return _context.SearchTitles.FromSqlInterpolated($"select * from string_search({searchTerm})").ToList();
        }

        //// FAKE list 
        private readonly List<User> _users = new List<User>         {
          new User { Id = 1, Username = "user1", Password = "password1", Email = "dsad"}};

    // --Name-- (Actors, Directors, Writers)
    public NameBasic GetNameByNConst(int userId, string nconst)
    {
        if (_users.FirstOrDefault(x => x.Id ==userId) == null) 
                throw new ArgumentException("User not found.");
            return _context.NameBasics.FirstOrDefault(p => p.NConst == nconst);
    }

    public IList<NameBasic> GetAllNames(int userId)
    {
         if (_users.FirstOrDefault(x => x.Id == userId) == null)
                throw new ArgumentException("User not found.");
            return _context.NameBasics.ToList();
    }


    public IList<TitleCharacter> GetTitleCharactersByName(string nconst)
    {
      return _context.TitleCharacters
                     .Where(tc => tc.NConst == nconst)
                     .ToList();
    }


    // --TITLE PRINCIPALS--
    public IList<TitlePrincipal> GetTitlePrincipalsByName(string nconst)
    {
      return _context.TitlePrincipals
                     .Where(tp => tp.NConst == nconst)
                     .ToList();
    }
    public IList<TitlePrincipal> GetTitlePrincipalsByTitle(string tconst)
    {
      return _context.TitlePrincipals
                     .Where(tp => tp.TConst == tconst)
                     .ToList();
    }


    // --KNOWN FOR TITLES--
    public IList<KnownForTitle> GetKnownForTitlesByName(string nconst)
    {
      return _context.KnownForTitles
                     .Where(k => k.NConst == nconst)
                     .ToList();
    }

  }
}