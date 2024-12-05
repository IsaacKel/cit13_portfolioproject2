// import React, { useEffect, useState } from "react";
// import { useDispatch, useSelector } from 'react-redux';
// import { setResults } from '../redux/searchSlice';
// import { useSearchParams } from "react-router-dom";
// import "./SearchResults.css";
// import {
//   fetchNamesSearch,
//   fetchTitlesSearch
// } from "../services/apiService";
// import Bookmark from "../components/Bookmark";
// import FiltersColumn from "../components/FiltersColumn";
// import ResultsColumn from "../components/ResultsColumn";

// const SearchResults = () => {
//   const [searchParams] = useSearchParams();
//   const query = searchParams.get("query");
//   const [names, setNames] = useState([]);
//   const [titles, setTitles] = useState([]);
//   const [loading, setLoading] = useState(true);

//   const [showBookmarkModal, setShowBookmarkModal] = useState(false);

//   const [titlesPage, setTitlesPage] = useState(1);
//   const [titlesTotalPages, setTitlesTotalPages] = useState(1);
//   const [namesPage, setNamesPage] = useState(1);
//   const [namesTotalPages, setNamesTotalPages] = useState(1);

//   const [showMoreTitles, setShowMoreTitles] = useState(false);
//   const [showMoreNames, setShowMoreNames] = useState(false);

//   const handlePageChange = (type, page) => {
//     if (type === "titles" && page >= 1 && page <= titlesTotalPages) {
//       setTitlesPage(page);
//       fetchTitlesSearch(
//         query,
//         page,
//         setLoading,
//         setTitles,
//         setTitlesTotalPages
//       );
//     } else if (type === "names" && page >= 1 && page <= namesTotalPages) {
//       setNamesPage(page);
//       fetchNamesSearch(query, page, setLoading, setNames, setNamesTotalPages);
//     }
//   };

//   const handleBookmarkClick = () => {
//     setShowBookmarkModal(true);
//   };

//   const [filters, setFilters] = useState({
//     sort: "rating",
//     genre: "",
//     year: "",
//     titleType: "",
//   });

//   const handleApplyFilters = (newFilters) => {
//     console.log("Applying filters search:", newFilters);
//     setFilters(newFilters);
//   };

//   const fetchData = async (query, namesPage, titlesPage) => {
//     console.log("fetchData called with:", {
//       query,
//       namesPage,
//       titlesPage,
//       filters,
//     });
//     setLoading(true);
//     try {
//       await fetchNamesSearch(
//         query,
//         namesPage,
//         setLoading,
//         setNames,
//         setNamesTotalPages
//       );
//       console.log("fetchNamesSearch completed");
//       if (filters.sort === "rating") {
//         await fetchTitlesByRating(
//           query,
//           titlesPage,
//           setLoading,
//           setTitles,
//           setTitlesTotalPages,
//           filters.titleType,
//           filters.genre,
//           filters.year
//         );
//       } else if (filters.sort === "popularity") {
//         await fetchTitlesByNumVotes(
//           query,
//           titlesPage,
//           setLoading,
//           setTitles,
//           setTitlesTotalPages,
//           filters.titleType,
//           filters.genre,
//           filters.year
//         );
//       } else {
//         await fetchTitlesSearch(
//           query,
//           titlesPage,
//           setLoading,
//           setTitles,
//           setTitlesTotalPages
//         );
//       }
//       console.log("fetchTitlesSearch completed");
//     } catch (error) {
//       console.error("Error fetching data:", error);
//     } finally {
//       setLoading(false);
//     }
//   };

//   useEffect(() => {
//     if (query) {
//       console.log(
//         "useEffect triggered with query:",
//         query,
//         "namesPage:",
//         namesPage,
//         "titlesPage:",
//         titlesPage,
//         "filters:",
//         filters
//       );
//       fetchData(query, namesPage, titlesPage);
//     }
//   }, [query, namesPage, titlesPage, filters]);

//   const getPaginationButtons = (totalPages, curPage) => {
//     const buttons = [];
//     const maxButtons = 5;

//     if (totalPages <= maxButtons) {
//       for (let i = 1; i <= totalPages; i++) {
//         buttons.push(i);
//       }
//     } else {
//       buttons.push(1);
//       let startPage = Math.max(curPage - 2, 2);
//       let endPage = Math.min(curPage + 2, totalPages - 1);

//       if (startPage > 2) {
//         buttons.push("...");
//       }

//       for (let i = startPage; i <= endPage; i++) {
//         buttons.push(i);
//       }

//       if (endPage < totalPages - 1) {
//         buttons.push("...");
//       }

//       buttons.push(totalPages);
//     }

//     return buttons;
//   };

//   if (loading) {
//     return <div>Searching...</div>;
//   }

//   return (
//     <div className="search-page">
//       <FiltersColumn onApplyFilters={handleApplyFilters} />
//       <ResultsColumn
//         query={query}
//         titles={titles}
//         names={names}
//         filters={filters}
//         showMoreTitles={showMoreTitles}
//         showMoreNames={showMoreNames}
//         setShowMoreTitles={setShowMoreTitles}
//         setShowMoreNames={setShowMoreNames}
//         handleBookmarkClick={handleBookmarkClick}
//         titlesPage={titlesPage}
//         titlesTotalPages={titlesTotalPages}
//         namesPage={namesPage}
//         namesTotalPages={namesTotalPages}
//         handlePageChange={handlePageChange}
//         getPaginationButtons={getPaginationButtons}
//       />
//       <Bookmark
//         show={showBookmarkModal}
//         onClose={() => setShowBookmarkModal(false)}
//       />
//     </div>
//   );
// };

// export default SearchResults;

import React, { useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import FiltersColumn from "../components/FiltersColumn";
import ResultsColumn from "../components/ResultsColumn";
import { useDispatch } from "react-redux";
import { setQuery, setFilters, setSortBy } from "../redux/slices/searchSlice";
import "./SearchResults.css";

const SearchPage = () => {
  const [searchParams] = useSearchParams();
  const dispatch = useDispatch();

  useEffect(() => {
    const query = searchParams.get("searchTerm") || "";
    const titleType = searchParams.get("titleType") || null;
    const genre = searchParams.get("genre") || null;
    const year = parseInt(searchParams.get("year")) || -1;
    const sortBy = searchParams.get("sortBy") || "popularity";

    dispatch(setQuery(query));
    dispatch(setFilters({ titleType, genre, year }));
    dispatch(setSortBy(sortBy));
  }, [searchParams, dispatch]);

  return (
    <div className="search-page">
      <div className="filters-column">
        <FiltersColumn />
      </div>
      <div className="results-column">
        <ResultsColumn />
      </div>
    </div>
  );
};

export default SearchPage;
