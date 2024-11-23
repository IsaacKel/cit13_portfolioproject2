import React, { useEffect, useState } from "react";
import { useSearchParams, Link } from "react-router-dom";

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
      <h3>Names</h3>
      <ul>
        {names.map((name, index) => (
          <li key={index}>
            <a href={name.nConst}>{name.primaryName}</a>
          </li>
        ))}
      </ul>
      <h3>Titles</h3>
      <ul>
        {titles.map((title, index) => (
          <li key={index}>
            <a href={title.tConst}>{title.primaryTitle}</a>
          </li>
        ))}
      </ul>
      {titles.map((title, index) => (
        <div key={index}>
          <Link to={`/title/${title.tConst.split("/").pop()}`}>
            {title.primaryTitle}
          </Link>
        </div>
      ))}
    </div>
  );
};

export default SearchResults;
