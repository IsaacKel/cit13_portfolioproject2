import React, { useEffect, useState } from "react";
import { useSearchParams, Link } from "react-router-dom";
import "./SearchResults.css";

const SearchResults = () => {
  const [searchParams] = useSearchParams();
  const query = searchParams.get("query");
  const [names, setNames] = useState([]);
  const [titles, setTitles] = useState([]);
  const [loading, setLoading] = useState(true);

  const [curPage, setCurPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  const [showAll, setShowAll] = useState(false);

  const handleShowMore = () => {
    setShowAll(true);
  };

  const handleShowLess = () => {
    setShowAll(false);
  };

  const handlePageChange = (page) => {
    setCurPage(page);
  };

  const displayedTitles = showAll ? titles : titles.slice(0, 3);

  useEffect(() => {
    if (query) {
      const fetchData = async () => {
        setLoading(true);
        try {
          const [nameRes, titleRes] = await Promise.all([
            fetch(`https://localhost:5003/api/Search/name/${query}`),
            fetch(`https://localhost:5003/api/Search/title/${query}`),
          ]);

          const nameData = await nameRes.json();
          const titleData = await titleRes.json();

          setNames(nameData.items || []);
          setTitles(titleData.items || []);
          setTotalPages(titleData.numberPages || 1);
        } catch (error) {
          console.error("Error fetching search results:", error);
        } finally {
          setLoading(false);
        }
      };

      fetchData();
    }
  }, [query]);

  if (loading) {
    return <div>Searching...</div>;
  }

  return (
    <div>
      <h2>Search Results for "{query}"</h2>
      <h3>Titles</h3>
      <div className="search-results-container">
        {titles.length === 0 && <p>No results found</p>}
        {displayedTitles.map((title, index) => (
          <Link
            to={`/title/${title.tConst.split("/").pop()}`}
            key={index}
            className="search-item-link"
          >
            <div className="search-item">
              {title.poster && (
                <img
                  src={title.poster}
                  alt={title.primaryTitle}
                  className="search-item-poster"
                />
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
            </div>
          </Link>
        ))}
      </div>
      {!showAll && titles.length > 4 && (
        <p onClick={handleShowMore} className="see-more-text">
          See more...
        </p>
      )}
      {showAll && (
        <p onClick={handleShowLess} className="see-more-text">
          See less...
        </p>
      )}
      <p>Total pages - need to add pagination: {totalPages}</p>
      <h3>People</h3>
      <div className="search-results-container">
        {names.length === 0 && <p>No people found</p>}
        {names.map((name, index) => (
          <Link
            to={`/name/${name.nConst.split("/").pop()}`}
            key={index}
            className="search-item-link"
          >
            <div className="search-item">
              <div className="search-item-title">{name.primaryName}</div>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
};

export default SearchResults;
