import React, { useState } from "react";
import "../pages/HomePage.css";

const Carousel = ({ items, itemsToShow = 8 }) => {
  const [startIndex, setStartIndex] = useState(0);

  const totalItems = items.length;

  const nextItems = () => {
    setStartIndex((prevIndex) => (prevIndex + 1) % totalItems);
  };

  const prevItems = () => {
    setStartIndex((prevIndex) => (prevIndex - 1 + totalItems) % totalItems);
  };

  const getVisibleItems = () => {
    return Array.from(
      { length: itemsToShow },
      (_, i) => items[(startIndex + i) % totalItems]
    );
  };

  return (
    <div className="carousel">
      <button className="carousel-button" onClick={prevItems}>
        &lt;
      </button>
      <div className="carousel-items">
        {getVisibleItems().map((item, index) => (
          <div className="carousel-item" key={index}>
            {item}
          </div>
        ))}
      </div>
      <button className="carousel-button" onClick={nextItems}>
        &gt;
      </button>
    </div>
  );
};

export default Carousel;
