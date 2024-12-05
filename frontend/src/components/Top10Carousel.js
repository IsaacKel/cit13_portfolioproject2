import React, { useState } from "react";
import { Link } from "react-router-dom";

const Carousel = ({ title, items, itemType }) => {
  const [currentIndex, setCurrentIndex] = useState(0);

  const handleLeftClick = () => {
    setCurrentIndex((prevIndex) => (prevIndex - 1 + 10) % 10);
  };

  const handleRightClick = () => {
    setCurrentIndex((prevIndex) => (prevIndex + 1) % 10);
  };

  const getVisibleItems = () => {
    const visibleItems = [];
    for (let i = 0; i < 8; i++) {
      visibleItems.push(items[(currentIndex + i) % 10]);
    }
    return visibleItems;
  };

  return (
    <div className="media-list">
      <h2>{title}</h2>
      <div className="carousel">
        <button onClick={handleLeftClick} className="carousel-button">
          &lt;
        </button>
        <div className="card-grid">
          {getVisibleItems().map((item) => (
            <Link
              to={`/${itemType}/${item.tConst || item.nConst}`}
              key={item.tConst || item.nConst}
              className="home-card"
            >
              <img
                src={item.poster || item.image}
                alt={itemType === "title" ? "Poster" : "Actor"}
                className="home-card-img"
              />
            </Link>
          ))}
        </div>
        <button onClick={handleRightClick} className="carousel-button">
          &gt;
        </button>
      </div>
    </div>
  );
};

export default Carousel;
