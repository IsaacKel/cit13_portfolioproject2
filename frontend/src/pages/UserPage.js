import React, { useEffect, useState } from "react";
import {
  fetchUserData,
  fetchBookmarks,
  fetchUserRatings,
  fetchUserSearchHistory,
} from "../services/apiService";
import { Link } from "react-router-dom";

const UserPage = () => {
  const [user, setUser] = useState(null);
  const [bookmarks, setBookmarks] = useState([]);
  const [ratings, setRatings] = useState([]);
  const [searchHistory, setSearchHistory] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

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
        setUser(userData);
        setBookmarks(userBookmarks);
        setRatings(userRatings);
        setSearchHistory(userSearchHistory);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []); // Remove userID from dependencies since it's no longer needed

  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <div>
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
                </>
              ) : (
                <>
                  <strong>Name:</strong>{" "}
                  <Link to={`/name/${bookmark.nConst}`}>{bookmark.nConst}</Link>
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
            </li>
          ))}
        </ul>
      ) : (
        <p>No bookmarks found.</p>
      )}

      <h2>Ratings</h2>
      {ratings.items && ratings.items.length > 0 ? (
        <ul>
          {ratings.items.map((rating) => (
            <li key={rating.id}>
              <p>
                <strong>Title:</strong>{" "}
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
                <strong>Search Query:</strong> {historyItem.searchQuery}
              </p>
              <p>
                <strong>ID:</strong> {historyItem.id}
              </p>
              <p>
                <strong>Date:</strong>{" "}
                {new Date(historyItem.createdAt).toLocaleDateString("en-GB")}
              </p>
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
