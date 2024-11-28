import React, { useEffect, useState } from "react";
import { useSearchParams, Link } from "react-router-dom";
import "./SearchResults.css";
import { fetchImages } from "../services/apiService";
import Bookmark from "../components/Bookmark";

const SearchResults = () => {
  const [searchParams] = useSearchParams();
  const query = searchParams.get("query");
  const [names, setNames] = useState([]);
  const [titles, setTitles] = useState([]);
  const [loading, setLoading] = useState(true);

  //open bookmark modal when bookmark is clicked
  const [showBookmarkModal, setShowBookmarkModal] = useState(false);

  const [curPage, setCurPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  const [showMoreTitles, setShowMoreTitles] = useState(false);
  const [showMorePeople, setShowMorePeople] = useState(false);

  const displayedTitles = showMoreTitles ? titles : titles.slice(0, 3);
  const displayedPeople = showMorePeople ? names : names.slice(0, 3);

  const handlePageChange = (page) => {
    if (page >= 1 && page <= totalPages) {
      setCurPage(page);
      fetchData(query, page); // Fetch results for the selected page
    }
  };

  const handleBookmarkClick = () => {
    setShowBookmarkModal(true);
  };

  const fetchData = async (query, pageNumber) => {
    setLoading(true);
    try {
      const [nameRes, titleRes] = await Promise.all([
        fetch(
          `http://localhost:5003/api/Search/name/${query}?pageNumber=${pageNumber}&pageSize=10`
        ),
        fetch(
          `http://localhost:5003/api/Search/title/${query}?pageNumber=${pageNumber}&pageSize=10`
        ),
      ]);

      const nameData = await nameRes.json();
      const titleData = await titleRes.json();

      // Fetch images for each person in the name data
      const namesWithImages = await Promise.all(
        (nameData.items || []).map(async (person) => {
          const imageUrl = await fetchImages(person.primaryName);
          return { ...person, imageUrl };
        })
      );

      setNames(namesWithImages);
      setTitles(titleData.items || []);
      setTotalPages(titleData.numberPages || 1);
    } catch (error) {
      console.error("Error fetching search results:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (query) {
      fetchData(query, curPage);
    }
  }, [query, curPage]);

  const getPaginationButtons = () => {
    const buttons = [];
    const maxButtons = 5; // Maximum number of buttons to display

    if (totalPages <= maxButtons) {
      // If total pages are less than or equal to maxButtons, show all pages
      for (let i = 1; i <= totalPages; i++) {
        buttons.push(i);
      }
    } else {
      // Always show the first page
      buttons.push(1);

      // Show the current page and up to two pages before and after it
      let startPage = Math.max(curPage - 2, 2);
      let endPage = Math.min(curPage + 2, totalPages - 1);

      // Add ellipsis if there's a gap between the first page and the startPage
      if (startPage > 2) {
        buttons.push("...");
      }

      // Add the range of pages around the current page
      for (let i = startPage; i <= endPage; i++) {
        buttons.push(i);
      }

      // Add ellipsis if there's a gap between the endPage and the last page
      if (endPage < totalPages - 1) {
        buttons.push("...");
      }

      // Always show the last page
      buttons.push(totalPages);
    }

    return buttons;
  };

  if (loading) {
    return <div>Searching...</div>;
  }

  return (
    <div className="search-page">
      {/* Filters Column */}
      <div className="filters-column">
        <h4>Filters</h4>
        <div>
          <label>
            <input type="checkbox" /> Movies
          </label>
        </div>
        <div>
          <label>
            <input type="checkbox" /> TV Shows
          </label>
        </div>
        <div>
          <label>
            <input type="checkbox" /> Actors
          </label>
        </div>
        <div>
          <label>
            <input type="checkbox" /> Directors
          </label>
        </div>
        <div>
          <label>Sort By:</label>
          <select>
            <option value="releaseDate">Release Date</option>
            <option value="rating">Rating</option>
            <option value="popularity">Popularity</option>
          </select>
        </div>
      </div>
      <div>
        <div className="results-column">
          <h2>Search Results for "{query}"</h2>
          <h3>Titles</h3>
          {titles.length === 0 && <p>No results found</p>}
          <div className="search-results-container">
            {displayedTitles.map((title, index) => (
              <div className="search-item">
                <Link
                  to={`/title/${title.tConst.split("/").pop()}`}
                  key={index}
                  className="search-item-link"
                >
                  {title.poster ? (
                    <img
                      src={title.poster}
                      alt={title.primaryTitle}
                      className="search-item-img"
                    />
                  ) : (
                    <div className="search-item-img placeholder"></div>
                  )}
                  <div className="search-item-title">{title.primaryTitle}</div>
                  <div className="search-item-details">
                    <p className="search-item-year">{title.startYear}</p>
                    <p className="search-item-genre">{title.genre}</p>
                  </div>
                  {title.rating && (
                    <div className="search-item-rating">
                      <span className="star">⭐</span>
                      <p className="title-rating">{title.rating}</p>
                    </div>
                  )}
                </Link>
                <button
                  className="add-to-bookmarks-button"
                  onClick={handleBookmarkClick}
                >
                  + Add to Bookmarks
                </button>
              </div>
            ))}
          </div>
          {titles.length > 3 && !showMoreTitles && (
            <p
              onClick={() => setShowMoreTitles(true)}
              className="see-more-text"
            >
              See more...
            </p>
          )}
          {showMoreTitles && (
            <p
              onClick={() => setShowMoreTitles(false)}
              className="see-more-text"
            >
              See less...
            </p>
          )}
        </div>
        {/* Pagination Controls */}
        <div className="pagination">
          <button
            onClick={() => handlePageChange(curPage - 1)}
            disabled={curPage === 1}
          >
            Previous
          </button>
          {getPaginationButtons().map((page, index) => (
            <button
              key={index}
              onClick={() => page !== "..." && handlePageChange(page)}
              className={curPage === page ? "active" : ""}
              disabled={page === "..."}
            >
              {page}
            </button>
          ))}
          <button
            onClick={() => handlePageChange(curPage + 1)}
            disabled={curPage === totalPages}
          >
            Next
          </button>
        </div>
        <h3>People</h3>
        <div className="search-results-container">
          {names.length === 0 && <p>No people found</p>}
          {displayedPeople.map((name, index) => (
            <div className="search-item">
              <Link
                to={`/name/${name.nConst.split("/").pop()}`}
                key={index}
                className="search-item-link"
              >
                {name.imageUrl ? (
                  <img
                    src={name.imageUrl}
                    alt={name.primaryName}
                    className="search-item-img"
                  />
                ) : (
                  <div className="search-item-img placeholder"></div>
                )}
                <div className="search-item-title">{name.primaryName}</div>
              </Link>
              <button
                className="add-to-bookmarks-button"
                onClick={handleBookmarkClick}
              >
                + Add to Bookmarks
              </button>
            </div>
          ))}
          {names.length > 3 && !showMorePeople && (
            <p
              onClick={() => setShowMorePeople(true)}
              className="see-more-text"
            >
              See more...
            </p>
          )}
          {showMorePeople && (
            <p
              onClick={() => setShowMorePeople(false)}
              className="see-more-text"
            >
              See less...
            </p>
          )}
        </div>
      </div>
      <Bookmark
        show={showBookmarkModal}
        onClose={() => setShowBookmarkModal(false)}
      />
    </div>
  );
};

export default SearchResults;
