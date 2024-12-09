import React, { createContext, useContext, useState, useEffect } from "react";
import {
  fetchBookmarks as fetchBookmarksFromApi,
  isTitleBookmarked as isTitleBookmarkedFromApi,
  addBookmark as addBookmarkToApi,
} from "../services/apiService";

const BookmarkContext = createContext();

export const BookmarkProvider = ({ children }) => {
  const [bookmarks, setBookmarks] = useState(new Set()); // Use a Set for fast lookups

  const fetchBookmarks = async () => {
    try {
      const token = localStorage.getItem("token");
      if (!token) {
        console.log("No token found in localStorage");
        return;
      }

      const userBookmarks = await fetchBookmarksFromApi();

      const tConstSet = new Set(
        userBookmarks.items.map((bookmark) => bookmark.tConst)
      );

      setBookmarks(tConstSet);
    } catch (err) {
      console.error("Error fetching bookmarks in Context:", err);
    }
  };

  const isBookmarked = async (tConst) => {
    try {
      const result = await isTitleBookmarkedFromApi(tConst);
      console.log("Checking if bookmarked:", tConst, "Result:", result);
      return result;
    } catch (err) {
      console.error("Error checking if title is bookmarked:", err);
      return false;
    }
  };

  useEffect(() => {
    fetchBookmarks();
  }, []);

  return (
    <BookmarkContext.Provider value={{ bookmarks, isBookmarked }}>
      {children}
    </BookmarkContext.Provider>
  );
};

export const useBookmarks = () => useContext(BookmarkContext);
