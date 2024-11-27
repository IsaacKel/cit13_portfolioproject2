import React, { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import "./IndividualTitle.css";
import {
  fetchImages,
  fetchTitleData,
  fetchSimilarTitles,
  fetchTitlePrincipals,
} from "../services/apiService";
import Bookmark from "../components/Bookmark";

const IndividualTitle = () => {
  const { tConst } = useParams();
  const [titleData, setTitleData] = useState(null);
  const [similarTitles, setSimilarTitles] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);
  const [principals, setPrincipals] = useState([]);
  const [castWithImages, setCastWithImages] = useState([]);
  const [showBookmarkModal, setShowBookmarkModal] = useState(false);

  const [crewPage, setCrewPage] = useState(0);
  const itemsPerPage = 8;
  const [similarTitlesPage, setSimilarTitlesPage] = useState(0);
  const similarTitlesPerPage = 8;

  const crewStartIndex = crewPage * itemsPerPage;
  const crewEndIndex = crewStartIndex + itemsPerPage;
  const currentCast = castWithImages.slice(crewStartIndex, crewEndIndex);

  const similarTitlesStartIndex = similarTitlesPage * similarTitlesPerPage;
  const similarTitlesEndIndex = similarTitlesStartIndex + similarTitlesPerPage;
  const currentSimilarTitles = similarTitles.slice(
    similarTitlesStartIndex,
    similarTitlesEndIndex
  );

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const data = await fetchTitleData(tConst);
        setTitleData(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [tConst]);

  useEffect(() => {
    const fetchSimilar = async () => {
      try {
        const data = await fetchSimilarTitles(tConst, 1, 10);
        if (data.items) setSimilarTitles(data.items);
      } catch (error) {
        console.error("Error fetching similar titles:", error);
      }
    };

    fetchSimilar();
  }, [tConst]);

  useEffect(() => {
    const fetchPrincipals = async () => {
      try {
        const data = await fetchTitlePrincipals(tConst);
        if (data.length === 0) {
          console.warn("No crew found for this title.");
        }
        setPrincipals(data);
      } catch (error) {
        console.error("Error fetching title principals:", error);
      }
    };

    fetchPrincipals();
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

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [tConst]);

  const formatTitleType = (type) => {
    return type === "tvShow" || "TvSeries"
      ? "TV Show"
      : type.charAt(0).toUpperCase() + type.slice(1);
  };

  const handleNextCrewPage = () => {
    if (crewEndIndex < castWithImages.length) {
      setCrewPage(crewPage + 1);
    }
  };

  const handlePrevCrewPage = () => {
    if (crewStartIndex > 0) {
      setCrewPage(crewPage - 1);
    }
  };

  const handleNextSimilarTitlesPage = () => {
    if (similarTitlesEndIndex < similarTitles.length) {
      setSimilarTitlesPage(similarTitlesPage + 1);
    }
  };

  const handlePrevSimilarTitlesPage = () => {
    if (similarTitlesStartIndex > 0) {
      setSimilarTitlesPage(similarTitlesPage - 1);
    }
  };

  const handleBookmarkClick = () => {
    setShowBookmarkModal(true);
  };

  const handleCloseBookmarkModal = () => {
    setShowBookmarkModal(false);
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
          {titleData.averageRating !== null && (
            <span className="rating">⭐ {titleData.averageRating}</span>
          )}{" "}
          <button>+ Add Rating</button>
          <button onClick={handleBookmarkClick}>+ Add to Bookmarks</button>
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
      <section className="cast-crew-similar-titles">
        <h2>Cast & Crew</h2>
        <div className="cards-list">
          {currentCast.length > 0 ? (
            currentCast.map((principal) => (
              <Link
                to={`/name/${principal.nConst}`}
                key={principal.nConst}
                className="search-item-link"
              >
                <div key={principal.nConst} className="card">
                  {principal.imageUrl ? (
                    <img
                      src={principal.imageUrl}
                      alt={principal.name}
                      className="card-img"
                    />
                  ) : (
                    <div className="card-img placeholder"></div>
                  )}
                  <div className="card-details">
                    <p>{principal.name}</p>
                    <p className="title-data">{principal.roles}</p>
                  </div>
                </div>
              </Link>
            ))
          ) : (
            <></>
          )}
        </div>
        <div className="pagination-buttons">
          <button onClick={handlePrevCrewPage} disabled={crewStartIndex === 0}>
            &lt; Prev
          </button>
          <button
            onClick={handleNextCrewPage}
            disabled={crewEndIndex >= castWithImages.length}
          >
            Next &gt;
          </button>
        </div>
      </section>

      {/* Similar Titles */}
      <section className="cast-crew-similar-titles">
        <h2>Similar Titles</h2>
        <div className="cards-list">
          {currentSimilarTitles.length > 0 ? (
            currentSimilarTitles.map((title) => (
              <Link
                to={`/title/${title.tConst.split("/").pop()}`}
                key={title.tConst}
                className="search-item-link"
              >
                <div key={title.tConst} className="card">
                  {title.poster ? (
                    <img
                      src={title.poster}
                      alt={title.primaryTitle}
                      className="card-img"
                    />
                  ) : (
                    <div className="card-img placeholder"></div>
                  )}
                  <div className="card-details">
                    <p>{title.primaryTitle}</p>
                  </div>
                </div>
              </Link>
            ))
          ) : (
            <></>
          )}
        </div>
        <div className="pagination-buttons">
          <button
            onClick={handlePrevSimilarTitlesPage}
            disabled={similarTitlesStartIndex === 0}
          >
            &lt; Prev
          </button>
          <button
            onClick={handleNextSimilarTitlesPage}
            disabled={similarTitlesEndIndex >= similarTitles.length}
          >
            Next &gt;
          </button>
        </div>
      </section>
      <Bookmark show={showBookmarkModal} onClose={handleCloseBookmarkModal} />
    </div>
  );
};

export default IndividualTitle;
