import React, { useEffect, useState } from "react";
import { useSearchParams, Link } from "react-router-dom";
import "./SearchResults.css";

const SearchResults = () => {
  const [searchParams] = useSearchParams();
  const query = searchParams.get("query");
  const [names, setNames] = useState([]);
  const [titles, setTitles] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (query) {
      const fetchData = async () => {
        setLoading(true);
        try {
          const [nameRes, titleRes] = await Promise.all([
            fetch(`http://localhost:5002/api/Search/name/${query}`),
            fetch(`http://localhost:5002/api/Search/title/${query}`),
          ]);

          const nameData = await nameRes.json();
          const titleData = await titleRes.json();

          setNames(nameData.items || []);
          setTitles(titleData.items || []);
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
        {titles.map((title, index) => (
          <Link
            to={`/title/${title.tConst.split("/").pop()}`}
            key={index}
            className="search-item-link"
          >
            <div className="search-item">
              <img
                src={title.poster}
                alt={title.primaryTitle}
                className="search-item-poster"
              />
              <div className="search-item-title">{title.primaryTitle}</div>
              <div className="search-item-details">
                <p>{title.startYear}</p>
                <p>{title.genre}</p>
              </div>
              <div className="search-item-rating">
                <span>⭐</span>
                <p>{title.rating}</p>
              </div>
            </div>
          </Link>
        ))}
      </div>
      <h3>People</h3>
      <ul>
        {names.map((name, index) => (
          <li key={index}>
            <a href={name.nConst}>{name.primaryName}</a>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default SearchResults;
