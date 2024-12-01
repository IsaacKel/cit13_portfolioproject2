import React, { useState, useEffect } from "react";
import {
  fetchGenres,
  fetchYears,
  fetchTitleTypes,
} from "../services/apiService";
import "./FiltersColumn.css";

const FiltersColumn = ({ onApplyFilters }) => {
  const [genres, setGenres] = useState([]);
  const [years, setYears] = useState([]);
  const [titleTypes, setTitleTypes] = useState([]);
  const [selectedSort, setSelectedSort] = useState("None");
  const [selectedGenre, setSelectedGenre] = useState("Select");
  const [selectedYear, setSelectedYear] = useState("Select");
  const [selectedTitleType, setSelectedTitleType] = useState("Select");

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [genresData, yearsData, titleTypesData] = await Promise.all([
          fetchGenres(),
          fetchYears(),
          fetchTitleTypes(),
        ]);
        setGenres(genresData);
        setYears(yearsData);
        setTitleTypes(titleTypesData);
      } catch (error) {
        console.error("Error fetching filter data:", error);
      }
    };

    fetchData();
  }, []);

  const handleApplyFilters = () => {
    const filters = {
      sort: selectedSort === "None" ? null : selectedSort,
      genre: selectedGenre === "Select" ? null : selectedGenre,
      year: selectedYear === "Select" ? -1 : selectedYear,
      titleType: selectedTitleType === "Select" ? null : selectedTitleType,
    };
    console.log("Applying filters:", filters);
    onApplyFilters(filters);
  };

  const handleClearFilters = () => {
    setSelectedSort("None");
    setSelectedGenre("Select");
    setSelectedYear("Select");
    setSelectedTitleType("Select");
  };

  return (
    <div className="filters-column">
      <h4>Filters</h4>
      <div>
        <label>Sort By:</label>
        <select
          className="dropdown"
          value={selectedSort}
          onChange={(e) => setSelectedSort(e.target.value)}
        >
          <option value="None">None</option>
          <option value="rating">Rating</option>
          <option value="popularity">Popularity</option>
        </select>
      </div>
      <div>
        <label>Genres:</label>
        <select
          className="dropdown"
          value={selectedGenre}
          onChange={(e) => setSelectedGenre(e.target.value)}
        >
          <option value="Select">Select</option>
          {genres.map((genre) => (
            <option key={genre.genre} value={genre.genre}>
              {genre.genre}
            </option>
          ))}
        </select>
      </div>
      <div>
        <label>Years:</label>
        <select
          className="dropdown"
          value={selectedYear}
          onChange={(e) => setSelectedYear(e.target.value)}
        >
          <option value="Select">Select</option>
          {years.map((year) => (
            <option key={year.startYear} value={year.startYear}>
              {year.startYear}
            </option>
          ))}
        </select>
      </div>
      <div>
        <label>Title Types:</label>
        <select
          className="dropdown"
          value={selectedTitleType}
          onChange={(e) => setSelectedTitleType(e.target.value)}
        >
          <option value="Select">Select</option>
          {titleTypes.map((type) => (
            <option key={type.titleType} value={type.titleType}>
              {type.titleType}
            </option>
          ))}
        </select>
      </div>
      <button onClick={handleApplyFilters} className="apply-filters">
        Apply Filters
      </button>
      <button onClick={handleClearFilters} className="clear-filters">
        Clear Filters
      </button>
    </div>
  );
};

export default FiltersColumn;
