// Home Page

import React, { useEffect, useState} from "react";
import { Link } from "react-router-dom";
import { fetchTop10Movies, fetchTop10TVShows, fetchTop10Actors, fetchImages } from "../services/apiService";
import "./IndividualTitle.css";
import { editorsChoice } from "../services/editorsChoice";

const HomePage = () => {
  const [top10Movies, setTop10Movies] = useState([]);
  const [top10TVShows, setTop10TVShows] = useState([]);
  const [top10Actors, setTop10Actors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [movies, tvShows, actors] = await Promise.all([
          fetchTop10Movies(),
          fetchTop10TVShows(),
          fetchTop10Actors(),
        ]);

        const actorsWithImages = await Promise.all(
          actors.map(async (actor) => {
            const image = await fetchImages(actor.nConst);
            return { ...actor, image };
          })
        );

        setTop10Movies(movies);
        setTop10TVShows(tvShows);
        setTop10Actors(actorsWithImages);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <>
    <h1 style={{textAlign: 'center'}}>Editors Choice</h1>
    <ul style={{ display: 'flex', justifyContent: 'center', listStyleType: 'none', padding: 0 }}>
        {editorsChoice.map((movie) => (
          <li className = 'card' key={movie.tConst} style={{ margin: '10px', width: '10%' }}>
            <h2>{movie.choice}</h2>
            <Link to={`/title/${movie.tConst}`} style={{ textDecoration: 'none', color: 'inherit' }}>
              <img src={movie.poster} alt={movie.primaryTitle} style={{ width: "100%", marginBottom: "10px" }} />
              <div style={{ textAlign: 'center', wordWrap: 'break-word' }}>{movie.primaryTitle}</div>
            </Link>
          </li>
        ))}
      </ul>
      <h2 style={{textAlign: 'center'}}>Top 10 Movies</h2>
      <ul style={{ display: 'flex'}}>
        {top10Movies.map((movie) => (
          <li  className = 'card' key={movie.tConst} style={{ margin: '10px', width: '10%' }}>
            <Link to={`/title/${movie.tConst}`} style={{ textDecoration: 'none', color: 'inherit' }}>
              <img src={movie.poster} alt={movie.primaryTitle} style={{ width: "100%", marginBottom: "10px" }} />
              <div style={{ textAlign: 'center', wordWrap: 'break-word' }}>{movie.primaryTitle}</div>
            </Link>
          </li>
        ))}
      </ul>
      <h2 style={{textAlign: 'center'}}>Top 10 TV Shows</h2>
      <ul style={{ display: 'flex'}}>
        {top10TVShows.map((show) => (
          <li className = 'card' key={show.tConst} style={{ margin: '10px', width: '10%' }}>
            <Link to={`/title/${show.tConst}`} style={{ textDecoration: 'none', color: 'inherit' }}>
              <img src={show.poster} alt={show.primaryTitle} style={{ width: "100%", marginBottom: "10px" }} />
              <div style={{ textAlign: 'center', wordWrap: 'break-word' }}>{show.primaryTitle}</div>
            </Link>
          </li>
        ))}
      </ul>
      <h2 style={{textAlign: 'center'}}>Top 10 Actors</h2>
      <ul style={{ display: 'flex'}}>
        {top10Actors.map((actor) => (
          <li className = 'card' key={actor.nConst} style={{ margin: '10px', width: '10%' }}>
            <Link to={`/name/${actor.nConst}`} style={{ textDecoration: 'none', color: 'inherit' }}>
              <img src={actor.image} alt={actor.primaryName} style={{ width: "100%", marginBottom: "10px" }} />
              <div style={{ textAlign: 'center', wordWrap: 'break-word' }}>{actor.primaryName}</div>
            </Link>
          </li>
        ))}
      </ul>
    </>
  );
};

export default HomePage;