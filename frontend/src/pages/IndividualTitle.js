import React, { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import "./IndividualTitle.css";
import { fetchImages } from "../services/apiService";

const IndividualTitle = () => {
  const { tConst } = useParams();
  const [titleData, setTitleData] = useState(null);
  const [similarTitles, setSimilarTitles] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);
  const [principals, setPrincipals] = useState([]);
  const [castWithImages, setCastWithImages] = useState([]);

  useEffect(() => {
    const fetchTitleData = async () => {
      try {
        const response = await fetch(
          `https://localhost:5003/api/Title/${tConst}`
        );
        if (!response.ok) throw new Error("Failed to fetch title data");
        const data = await response.json();
        setTitleData(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchTitleData();
  }, [tConst]);

  useEffect(() => {
    fetch(
      `https://localhost:5003/api/SimilarMovies/${tConst}?pageNumber=1&pageSize=10`
    )
      .then((response) => response.json())
      .then((data) => {
        if (data.items) setSimilarTitles(data.items);
      })
      .catch((error) => console.error("Error fetching similar titles:", error));
  }, [tConst]);

  useEffect(() => {
    fetch(`https://localhost:5003/api/TitlePrincipal/${tConst}/principals`)
      .then((response) => {
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then((data) => {
        if (data.length === 0) {
          console.warn("No crew found for this title.");
        }
        setPrincipals(data);
      })
      .catch((error) =>
        console.error("Error fetching title principals:", error)
      );
  }, [tConst]);

  useEffect(() => {
    const fetchCastImages = async () => {
      const updatedPrincipals = await Promise.all(
        principals.map(async (principal) => {
          const imageUrl = await fetchImages(principal.name);
          return { ...principal, imageUrl };
        })
      );
      setCastWithImages(updatedPrincipals);
    };

    fetchCastImages();
  }, [principals]);

  const formatTitleType = (type) => {
    return type === "tvShow" || "TvSeries"
      ? "TV Show"
      : type.charAt(0).toUpperCase() + type.slice(1);
  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <div className="individual-title-container">
      {/* Top Section */}
      <div className="title-header">
        <div className="title-data">
          <h1>{titleData.primaryTitle}</h1>
          <div className="meta-data">
            <span>{formatTitleType(titleData.titleType)}</span>
            {titleData.startYear && <span>{titleData.startYear}</span>}
            {titleData.genres && <span>{titleData.genres.join(", ")}</span>}
            {titleData.runTimeMinutes && (
              <span>{titleData.runTimeMinutes} min</span>
            )}
          </div>
        </div>
        <div className="title-actions">
          <span className="rating">⭐ {titleData.averageRating}</span>
          <button>+ Add Rating</button>
          <button>+ Add to Bookmarks</button>
        </div>
      </div>

      {/* Poster and Plot */}
      <div className="poster-plot-container">
        {titleData.poster && (
          <img
            src={titleData.poster}
            alt={`${titleData.primaryTitle} poster`}
            className="poster"
          />
        )}
        <p className="plot">{titleData.plot}</p>
      </div>

      {/* Cast & Crew */}
      <section className="similar-titles">
        <h2>Cast & Crew</h2>
        <div className="similar-titles-list">
          {castWithImages.length > 0 ? (
            castWithImages.map((principal) => (
              <Link
                to={`/name/${principal.nConst}`}
                key={principal.nConst}
                className="search-item-link"
              >
                <div key={principal.nConst} className="cast-card">
                  {principal.imageUrl && (
                    <img
                      src={principal.imageUrl}
                      alt={principal.name}
                      className="cast-card-img"
                    />
                  )}
                  <div className="cast-card-details">
                    <p>{principal.name}</p>
                    <p>{principal.category}</p>
                  </div>
                </div>
              </Link>
            ))
          ) : (
            <></>
          )}
        </div>
      </section>

      {/* Similar Titles */}
      <section className="similar-titles">
        <h2>Similar Titles</h2>
        <div className="similar-titles-list">
          {similarTitles.length > 0 ? (
            similarTitles.map((title) => (
              <Link
                to={`/title/${title.tConst.split("/").pop()}`}
                key={title.tConst}
                className="search-item-link"
              >
                <img src={title.poster} alt={title.primaryTitle} />
                <p>{title.primaryTitle}</p>
              </Link>
            ))
          ) : (
            <></>
          )}
        </div>
      </section>
    </div>
  );
};

export default IndividualTitle;
