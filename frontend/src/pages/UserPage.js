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
import "./UserPage.css";

const UserPage = () => {
  const navigate = useNavigate();
  const { isLoggedIn, loadingAuth, logout } = useContext(AuthContext);
  const [user, setUser] = useState(null);
  const [bookmarks, setBookmarks] = useState([]);
  const [ratings, setRatings] = useState([]);
  const [searchHistory, setSearchHistory] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedSection, setSelectedSection] = useState("user-details");

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
        setRatings(ratingsWithTitles || []);
      } catch (err) {
        setRatings([]);
      }

      try {
        const userSearchHistory = await fetchUserSearchHistory();
        setSearchHistory(userSearchHistory || []);
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
        items: prevBookmarks.items.filter((b) => b.id !== bookmarkId),
      }));
    } catch (err) {
      setError(err.message);
    }
  };

  const handleDeleteSearchHistory = async (searchHistoryId) => {
    try {
      await deleteSearchHistory(searchHistoryId);
      setSearchHistory((prevSearchHistory) => ({
        items: prevSearchHistory.items.filter((s) => s.id !== searchHistoryId),
      }));
    } catch (err) {
      setError(err.message);
    }
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <div className="container-fluid">
      <nav className="sidebar">
        <ul className="nav flex-column">
          <li className="nav-item">
            <a
              className={`nav-link ${
                selectedSection === "user-details" ? "active" : ""
              }`}
              href="#user-details"
              onClick={() => setSelectedSection("user-details")}
            >
              User Details
            </a>
          </li>
          <li className="nav-item">
            <a
              className={`nav-link ${
                selectedSection === "bookmarks" ? "active" : ""
              }`}
              href="#bookmarks"
              onClick={() => setSelectedSection("bookmarks")}
            >
              Bookmarks
            </a>
          </li>
          <li className="nav-item">
            <a
              className={`nav-link ${
                selectedSection === "ratings" ? "active" : ""
              }`}
              href="#ratings"
              onClick={() => setSelectedSection("ratings")}
            >
              Ratings
            </a>
          </li>
          <li className="nav-item">
            <a
              className={`nav-link ${
                selectedSection === "search-history" ? "active" : ""
              }`}
              href="#search-history"
              onClick={() => setSelectedSection("search-history")}
            >
              Search History
            </a>
          </li>
        </ul>
      </nav>
      <div className="main-content">
        {selectedSection === "user-details" && (
          <div>
            <h2>Your Details</h2>
            {user && (
              <>
                <p>
                  <strong>Email:</strong> {user.email}
                </p>
                <p>
                  <strong>Username:</strong> {user.username}
                </p>
              </>
            )}
          </div>
        )}
        {selectedSection === "bookmarks" && (
          <>
            <h2>Bookmarks</h2>
            {bookmarks.items && bookmarks.items.length > 0 ? (
              <div className="user-card-grid">
                {bookmarks.items.map((bookmark) => (
                  <div className="user-card" key={bookmark.id}>
                    <div className="user-card-content">
                      {bookmark.tConst ? (
                        <Link to={`/title/${bookmark.tConst}`}>
                          <>
                            <div className="user-card-title">
                              <p>{bookmark.title.primaryTitle}</p>
                            </div>
                            <img src={bookmark.title.poster} alt="poster" />
                          </>
                        </Link>
                      ) : (
                        <Link to={`/name/${bookmark.nConst}`}>
                          <>
                            <div className="user-card-title">
                              <p>{bookmark.name.actualName}</p>
                            </div>
                            <img src={bookmark.image} alt="person" />
                          </>
                        </Link>
                      )}
                      <p>
                        <strong>Note:</strong> {bookmark.note}
                      </p>
                      <p>
                        <strong>Date:</strong>{" "}
                        {new Date(bookmark.createdAt).toLocaleDateString(
                          "en-GB"
                        )}
                      </p>
                    </div>
                    <button onClick={() => handleDeleteBookmark(bookmark.id)}>
                      Delete Bookmark
                    </button>
                  </div>
                ))}
              </div>
            ) : (
              <p>No bookmarks found.</p>
            )}
          </>
        )}

        {selectedSection === "ratings" && (
          <>
            <h2>Ratings</h2>
            {ratings && ratings.length > 0 ? (
              <ul>
                {ratings.map((rating) => (
                  <li key={rating.id}>
                    <p>
                      <Link to={`/title/${rating.tConst}`}>
                        {rating.title.primaryTitle}
                      </Link>
                    </p>
                    <p>
                      <strong>Rating:</strong> {rating.rating}
                    </p>
                    <p>
                      <strong>Date:</strong>{" "}
                      {new Date(rating.createdAt).toLocaleDateString("en-GB")}
                    </p>
                    <p>
                      <strong>Title Type:</strong> {rating.title.titleType}
                    </p>
                    <p>
                      <img
                        src={rating.title.poster}
                        alt="poster"
                        style={{ width: "100px", height: "auto" }}
                      />
                    </p>
                    <button
                      onClick={() =>
                        handleDeleteRating(rating.userId, rating.id)
                      }
                    >
                      Delete Rating
                    </button>
                  </li>
                ))}
              </ul>
            ) : (
              <p>No ratings found.</p>
            )}
          </>
        )}
        {selectedSection === "search-history" && (
          <>
            <h2>Search History</h2>
            {searchHistory.items && searchHistory.items.length > 0 ? (
              <ul>
                {searchHistory.items.map((historyItem) => (
                  <li key={historyItem.id}>
                    <p>
                      <strong>Query:</strong>{" "}
                      <Link
                        to={`/search?searchTerm=${encodeURIComponent(
                          historyItem.searchQuery
                        )}`}
                      >
                        {historyItem.searchQuery}
                      </Link>
                    </p>
                    <p>
                      <strong>Date:</strong>{" "}
                      {new Date(historyItem.createdAt).toLocaleDateString(
                        "en-GB"
                      )}
                    </p>
                    <button
                      onClick={() => handleDeleteSearchHistory(historyItem.id)}
                    >
                      Delete Search History
                    </button>
                  </li>
                ))}
              </ul>
            ) : (
              <p>No search history found.</p>
            )}
          </>
        )}
      </div>
    </div>
  );
};

export default UserPage;
