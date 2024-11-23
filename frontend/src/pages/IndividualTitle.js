import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const IndividualTitle = () => {
  const { tConst } = useParams();
  const [titleData, setTitleData] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Fetch the title data
    const fetchTitleData = async () => {
      try {
        const response = await fetch(
          `http://localhost:5002/api/Title/${tConst}`
        );
        if (!response.ok) {
          throw new Error("Failed to fetch title data");
        }
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

  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error}</p>;

  return (
    <div>
      <h1>{titleData.primaryTitle}</h1>
      <p>
        <strong>Plot:</strong> {titleData.plot}
      </p>
      <p>
        <strong>Release Date:</strong> {titleData.releaseDate}
      </p>
      <p>
        <strong>Runtime:</strong> {titleData.runTimeMinutes} minutes
      </p>
      <p>
        <strong>Start Year:</strong> {titleData.startYear}
      </p>
      <p>
        <strong>Type:</strong> {titleData.titleType}
      </p>
      {titleData.poster && (
        <img src={titleData.poster} alt={titleData.primaryTitle} />
      )}
    </div>
  );
};

export default IndividualTitle;
