import React, { useEffect, useState } from "react";
import { fetchUserData, fetchUserBookmarks } from "../services/apiService";
import { Link } from "react-router-dom";

const UserPage = () => {
  const userID = 85;
  const [user, setUser] = useState(null);
  const [bookmarks, setBookmarks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [userData, userBookmarks] = await Promise.all([
          fetchUserData(userID),
          fetchUserBookmarks(userID),
        ]);
        setUser(userData);
        setBookmarks(userBookmarks);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [userID]);

  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <div>
      <h1>User Page</h1>
      {user && (
        <div>
          <p>ID: {user.id}</p>
          <p>Email: {user.email}</p>
          <p>Username: {user.username}</p>
          {/* Add more user fields as needed */}
        </div>
      )}
      <h2>Bookmarks</h2>
      {bookmarks.items && bookmarks.items.length > 0 ? (
        <ul>
          {bookmarks.items.map((bookmark) => (
            <li key={bookmark.tConst || bookmark.nConst}>
            {bookmark.tConst ? (
              <>
                <strong>Title:</strong> <Link to={`/title/${bookmark.tConst}`}>{bookmark.tConst}</Link>
              </>
            ) : (
              <>
                <strong>Name:</strong> <Link to={`/name/${bookmark.nConst}`}>{bookmark.nConst}</Link>
              </>
            )}
            {<p>Note: {bookmark.note}</p>}
            {<p>Date: {bookmark.createdAt}</p>}
          </li>
          ))}
        </ul>
      ) : (
        <p>No bookmarks found.</p>
      )}
      <h2>Ratings (not done)</h2>
      <h2>Search History (not done)</h2>
    </div>
  );
};

export default UserPage;