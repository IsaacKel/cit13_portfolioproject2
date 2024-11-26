// Home Page

import React, { useEffect, useState } from "react";
import { fetchTop10Movies, fetchTop10TVShows, fetchTop10Actors, fetchImages } from "../services/apiService";

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
            const image = await fetchImages(actor.primaryName);
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
      <h1>Home Page</h1>
      <p>Welcome to the Home Page</p>
      <h2>Top 10 Movies</h2>
      <ul>
        {top10Movies.map((movie) => (
          <li key={movie.tConst}>
            <img src={movie.poster} alt={movie.primaryTitle} style={{ width: "50px", marginRight: "10px" }} />
            {movie.primaryTitle}
          </li>
        ))}
      </ul>
      <h2>Top 10 TV Shows</h2>
      <ul>
        {top10TVShows.map((show) => (
          <li key={show.tConst}>
            <img src={show.poster} alt={show.primaryTitle} style={{ width: "50px", marginRight: "10px" }} />
            {show.primaryTitle}
          </li>
        ))}
      </ul>
      <h2>Top 10 Actors</h2>
      <ul>
      {top10Actors.map((actor) => (
          <li key={actor.nConst}>
            <img src={actor.image} alt={actor.primaryName} style={{ width: "50px", marginRight: "10px" }} />
            {actor.primaryName}
          </li>
        ))}
      </ul>
    </>
  );
};

export default HomePage;