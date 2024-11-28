import React from "react";

const Filters = () => {
  return (
    <div className="filters-column">
      <h4>Filters</h4>
      {["Movies", "TV Shows", "Actors", "Directors"].map((filter) => (
        <div key={filter}>
          <label>
            <input type="checkbox" /> {filter}
          </label>
        </div>
      ))}
      <div>
        <label>Sort By:</label>
        <select>
          <option value="releaseDate">Release Date</option>
          <option value="rating">Rating</option>
        </select>
      </div>
    </div>
  );
};

export default Filters;