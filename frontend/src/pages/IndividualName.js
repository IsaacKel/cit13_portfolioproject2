import React, { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import "./IndividualTitle.css";
import {
  fetchImages,
  fetchNameData,
  fetchKnownForTitles,
  fetchCoPlayers,
  fetchPrincipalsByName,
} from "../services/apiService";
import Bookmark from "../components/Bookmark";
import PaginationButtons from "../components/PaginationButtons";
import CardList from "../components/CardList";
import { Card } from "react-bootstrap";

const ITEMS_PER_PAGE = 8;

const IndividualName = () => {
  const { nConst } = useParams();
  const [nameData, setNameData] = useState(null);
  const [knownFor, setKnownFor] = useState([]);
  const [knownForPage, setKnownForPage] = useState(0);
  const [coPlayers, setCoPlayers] = useState([]);
  const [principals, setPrincipals] = useState([]);
  const [showBookmarkModal, setShowBookmarkModal] = useState(false);
  const [personImage, setPersonImage] = useState(null);
  const [coPlayersWithImages, setCoPlayersWithImages] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        const [name, known, coPlayers, principals] = await Promise.all([
          fetchNameData(nConst),
          fetchKnownForTitles(nConst, 1, ITEMS_PER_PAGE * 5),
          fetchCoPlayers(nConst),
          fetchPrincipalsByName(nConst),
        ]);

        setNameData(name);
        setKnownFor(known.items || []);
        setCoPlayers(coPlayers || []);
        setPrincipals(principals || []);

        const imageUrl = await fetchImages(name.actualName);
        setPersonImage(imageUrl);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
    setKnownForPage(0);
    window.scrollTo(0, 0);
  }, [nConst]);

  //fetch costar image

  useEffect(() => {
    const fetchCostarImages = async () => {
      if (!coPlayers || !coPlayers.items || !coPlayers.items.length) {
        console.log("coPlayers or items not ready yet");
        return;
      }
      console.log("coPlayers", coPlayers);
      console.log("coPlayers.items", coPlayers.items);
      console.log("coPlayers.items.length", coPlayers.items.length);

      try {
        const updatedCoPlayers = await Promise.all(
          coPlayers.items.map(async (coPlayer) => {
            const imageUrl = await fetchImages(coPlayer.primaryName);
            return { ...coPlayer, imageUrl };
          })
        );
        setCoPlayersWithImages(updatedCoPlayers);
        console.log("updatedCoPlayers", updatedCoPlayers);
      } catch (err) {
        console.error("Error fetching coPlayer images:", err);
      }
    };

    fetchCostarImages();
  }, [coPlayers]);

  console.log("coPlayersWithImages", coPlayersWithImages);

  return (
    <div className="IndividualName">
      {loading ? (
        <p>Loading...</p>
      ) : error ? (
        <p>{error}</p>
      ) : (
        <>
          <h1>{nameData.actualName}</h1>
          {personImage && <img src={personImage} alt={nameData.actualName} />}
          <p>
            {nameData.birthYear}
            {nameData.deathYear ? ` - ${nameData.deathYear}` : ""}
          </p>
          <h2>Known For</h2>
          <section className="cast-crew-similar-titles">
            <CardList
              items={knownFor.slice(
                knownForPage * ITEMS_PER_PAGE,
                (knownForPage + 1) * ITEMS_PER_PAGE
              )}
              renderItem={(title) => (
                <Link
                  to={`/title/${title.knownForTitles.split("/").pop()}`}
                  key={title.knownForTitles}
                  className="search-item-link"
                >
                  <div className="card">
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
              )}
            />
          </section>
          <h2>Costars</h2>
          <section className="cast-crew-similar-titles">
            <CardList
              items={coPlayersWithImages.slice(
                knownForPage * ITEMS_PER_PAGE,
                (knownForPage + 1) * ITEMS_PER_PAGE
              )}
              renderItem={(coPlayer) => (
                <Link
                  to={`/name/${coPlayer.primaryName}`}
                  key={coPlayer.primaryName}
                  className="search-item-link"
                >
                  <div className="card">
                    {coPlayer.imageUrl ? (
                      <img
                        src={coPlayer.imageUrl}
                        alt={coPlayer.primaryName}
                        className="card-img"
                      />
                    ) : (
                      <div className="card-img placeholder"></div>
                    )}
                    <div className="card-details">
                      <p>{coPlayer.primaryName}</p>
                      <p>{coPlayer.frequency} times</p>
                    </div>
                  </div>
                </Link>
              )}
            />
          </section>
          <h2>Principals</h2>
          <section className="cast-crew-similar-titles">
            <CardList
              items={principals.slice(
                knownForPage * ITEMS_PER_PAGE,
                (knownForPage + 1) * ITEMS_PER_PAGE
              )}
              renderItem={(principal) => (
                <Link
                  to={`/title/${principal.tConst}`}
                  key={principals.tConst}
                  className="search-item-link"
                >
                  <div className="card">
                    {principal.imageUrl ? (
                      <img
                        src={principal.imageUrl}
                        alt={principal.title}
                        className="card-img"
                      />
                    ) : (
                      <div className="card-img placeholder"></div>
                    )}
                    <div className="card-details">
                      <p>{principal.title}</p>
                      <p className="title-data">{principal.roles}</p>
                    </div>
                  </div>
                </Link>
              )}
            />
          </section>
        </>
      )}
    </div>
  );
};

export default IndividualName;
