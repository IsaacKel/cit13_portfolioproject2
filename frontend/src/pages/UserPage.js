import React, { useEffect, useState, useContext } from "react";
import {
  fetchUserData,
  fetchBookmarks,
  fetchUserRatings,
  fetchUserSearchHistory,
  fetchTitleData,
  fetchNameData,
  fetchImages,
  deleteRating,
  deleteBookmark,
  deleteSearchHistory,
} from "../services/apiService";
import { Link, useNavigate } from "react-router-dom";
import AuthContext from "../components/AuthContext";

const UserPage = () => {
  const navigate = useNavigate();
  const { isLoggedIn, loadingAuth, logout } = useContext(AuthContext);
  const [user, setUser] = useState(null);
  const [bookmarks, setBookmarks] = useState([]);
  const [ratings, setRatings] = useState([]);
  const [searchHistory, setSearchHistory] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

   useEffect(() => {
    if (!loadingAuth && !isLoggedIn) {
      navigate("/login"); // Redirect to the login page if not logged in
    }
  }, [isLoggedIn, navigate]);


  useEffect(() => {
    const fetchData = async () => {
      try {
        const [userData, userBookmarks, userRatings, userSearchHistory] =
          await Promise.all([
            fetchUserData(),
            fetchBookmarks(),
            fetchUserRatings(),
            fetchUserSearchHistory(),
          ]);
  
        setUser(userData || {});
  
        const bookmarksWithTitles = await Promise.all(
          userBookmarks.items.map(async (bookmark) => {
            if (bookmark.tConst) {
              const titleData = await fetchTitleData(bookmark.tConst);
              return { ...bookmark, title: titleData };
            } else if (bookmark.nConst) {
              const nameData = await fetchNameData(bookmark.nConst);
              const nameImage = await fetchImages(bookmark.nConst);
              return { ...bookmark, name: nameData, image: nameImage };
            }
            return bookmark;
          })
        );
        setBookmarks({ items: bookmarksWithTitles });
      } catch (err) {
        console.error(err);
      }
  
      try {
        const userRatings = await fetchUserRatings();
        const ratingsWithTitles = await Promise.all(
          userRatings.items.map(async (rating) => {
            const titleData = await fetchTitleData(rating.tConst);
            return { ...rating, title: titleData };
          })
        );
        //console.log(ratingsWithTitles);
        setRatings(ratingsWithTitles || []);
      } catch (err) {
        setRatings([]);
      }
  
      try {
        const userSearchHistory = await fetchUserSearchHistory();
        setSearchHistory(userSearchHistory || []);
        //console.log(userSearchHistory);
      } catch (err) {
        setSearchHistory([]);
      }
  
      setLoading(false);
    };
  
    fetchData();
  }, []);

  const handleDeleteRating = async (userId, ratingId) => {
    try {
      await deleteRating(userId, ratingId);
      setRatings((prevRatings) => prevRatings.filter((r) => r.id !== ratingId));
    } catch (err) {
      setError(err.message);
    }
  };
  const handleDeleteBookmark = async (bookmarkId) => {
    try {
      await deleteBookmark(bookmarkId);
      setBookmarks((prevBookmarks) => ({
        items: prevBookmarks.items.filter((b) => b.id !== bookmarkId)
      }));
    } catch (err) {
      setError(err.message);
    }
  };
  const handleDeleteSearchHistory = async (searchHistoryId) => {
    try {
      await deleteSearchHistory(searchHistoryId);
      setSearchHistory((prevSearchHistory) => ({ items: prevSearchHistory.items.filter((s) => s.id !== searchHistoryId) }));
    } catch (err) {
      setError(err.message);
    }
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <div style={{color: 'white'}}>
      <h1>User Page</h1>
      {user && (
        <div>
          <p>
            <strong>ID:</strong> {user.id}
          </p>
          <p>
            <strong>Email:</strong> {user.email}
          </p>
          <p>
            <strong>Username:</strong> {user.username}
          </p>
          {/* Add more user fields as needed */}
        </div>
      )}

      <h2>Bookmarks</h2>
      {bookmarks.items && bookmarks.items.length > 0 ? (
        <ul>
          {bookmarks.items.map((bookmark) => (
            <li key={bookmark.id}>
              {bookmark.tConst ? (
                <>
                  <strong>Title:</strong>{" "}
                  <Link to={`/title/${bookmark.tConst}`}>
                    {bookmark.tConst}
                  </Link>
                  <p>
                    <strong>Title Name:</strong> {bookmark.title.primaryTitle}
                  </p>
                  <p>
                    <strong>Title Type:</strong> {bookmark.title.titleType}
                  </p>
                  <p>
                    <strong>Poster: </strong>{<img src={bookmark.title.poster} alt="poster" style={{ width: '100px', height: 'auto' }} />}
                  </p>
                </>
              ) : (
                <>
                  <strong>Name:</strong>{" "}
                  <Link to={`/name/${bookmark.nConst}`}>{bookmark.nConst}</Link>
                  <p>
                    <strong>Name:</strong> {bookmark.name.actualName}
                  </p>
                  <p>
                    <strong>Birth Year:</strong>{bookmark.name.birthYear}
                  </p>
                  <p>
                    <strong>Death Year:</strong>{bookmark.name.deathYear}
                  </p>
                  <p>
                    <string>Rating: </string>{bookmark.name.nRating}
                  </p>
                  <p>
                    <strong>Image: </strong>{<img src={bookmark.image} alt="image" style={{ width: '100px', height: 'auto' }} />}
                  </p>
                </>
              )}
              <p>
                <strong>Note:</strong> {bookmark.note}
              </p>
              <p>
                <strong>Date:</strong>{" "}
                {new Date(bookmark.createdAt).toLocaleDateString("en-GB")}
              </p>
              <p>
                <strong>ID:</strong> {bookmark.id}
              </p>
              <button onClick={() => handleDeleteBookmark(bookmark.id)}> Delete Bookmark</button>
            </li>
          ))}
        </ul>
      ) : (
        <p>No bookmarks found.</p>
      )}

      <h2>Ratings</h2>
      {ratings && ratings.length > 0 ? (
        <ul>
          {ratings.map((rating) => (
            <li key={rating.id}>
              <p>
                <strong>Title ID:</strong>{" "}
                <Link to={`/title/${rating.tConst}`}>{rating.tConst}</Link>
              </p>
              <p>
                <strong>Rating:</strong> {rating.rating}
              </p>
              <p>
                <strong>ID:</strong> {rating.id}
              </p>
              <p>
                <strong>Date:</strong>{" "}
                {new Date(rating.createdAt).toLocaleDateString("en-GB")}
              </p>
              <p>
                <strong>Title Name:</strong> {rating.title.primaryTitle}
              </p>
              <p>
                <strong>Title Type:</strong> {rating.title.titleType}
              </p>
              <p>
                <strong>Poster: </strong>{<img src={rating.title.poster} alt="poster" style={{ width: '100px', height: 'auto' }} />}
              </p>
              <button onClick={() => handleDeleteRating(rating.userId, rating.id)}> Delete Rating</button>
            </li>
          ))}
        </ul>
      ) : (
        <p>No ratings found.</p>
      )}

      <h2>Search History</h2>
      {searchHistory.items && searchHistory.items.length > 0 ? (
        <ul>
          {searchHistory.items.map((historyItem) => (
            <li key={historyItem.id}>
              <p>
                <strong>Search Query:</strong> {" "}
                <Link to={`/search?searchTerm=${encodeURIComponent(historyItem.searchQuery)}`}>
                {historyItem.searchQuery}
              </Link>
              </p>
              <p>
                <strong>ID:</strong> {historyItem.id}
              </p>
              <p>
                <strong>Date:</strong>{" "}
                {new Date(historyItem.createdAt).toLocaleDateString("en-GB")}
              </p>
              <button onClick={() => handleDeleteSearchHistory(historyItem.id)}> Delete Search History</button>
            </li>
          ))}
        </ul>
      ) : (
        <p>No search history found.</p>
      )}
    </div>
  );
};

export default UserPage;
