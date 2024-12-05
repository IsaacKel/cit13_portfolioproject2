import React, { useEffect, useState } from "react";
import {
  fetchTop10Movies,
  fetchTop10TVShows,
  fetchTop10Actors,
  fetchImages,
} from "../services/apiService";
import Carousel from "../components/Top10Carousel";
import "./HomePage.css";

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
    <div className="homepage">
      <h1>Editor's Choice</h1>
      <p>Placeholder for editor's choice</p>

      <h2>Top 10 Movies</h2>
      <Carousel
        items={top10Movies.map((movie) => (
          <div className="card" key={movie.tConst}>
            <img
              src={movie.poster}
              alt={movie.primaryTitle}
              className="card-img"
            />
            <p>{movie.primaryTitle}</p>
          </div>
        ))}
      />

      <h2>Top 10 TV Shows</h2>
      <Carousel
        items={top10TVShows.map((show) => (
          <div className="card" key={show.tConst}>
            <img
              src={show.poster}
              alt={show.primaryTitle}
              className="card-img"
            />
            <p>{show.primaryTitle}</p>
          </div>
        ))}
      />

      <h2>Top 10 Actors</h2>
      <Carousel
        items={top10Actors.map((actor) => (
          <div className="card" key={actor.nConst}>
            <img
              src={actor.image}
              alt={actor.primaryName}
              className="card-img"
            />
            <p>{actor.primaryName}</p>
          </div>
        ))}
      />
    </div>
  );
};

export default HomePage;
