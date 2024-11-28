import React from "react";

const FiltersColumn = () => (
  <div className="filters-column">
    <h4>Filters</h4>
    <div>
      <label>
        <input type="checkbox" /> Movies
      </label>
    </div>
    <div>
      <label>
        <input type="checkbox" /> TV Shows
      </label>
    </div>
    <div>
      <label>
        <input type="checkbox" /> Actors
      </label>
    </div>
    <div>
      <label>
        <input type="checkbox" /> Directors
      </label>
    </div>
    <div>
      <label>Sort By:</label>
      <select>
        <option value="releaseDate">Release Date</option>
        <option value="rating">Rating</option>
        <option value="popularity">Popularity</option>
      </select>
    </div>
  </div>
);

export default FiltersColumn;
