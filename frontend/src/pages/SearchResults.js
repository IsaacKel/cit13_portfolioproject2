import React, { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import "./SearchResults.css";
import { fetchNamesSearch, fetchTitlesSearch } from "../services/apiService";
import Bookmark from "../components/Bookmark";
import FiltersColumn from "../components/FiltersColumn";
import ResultsColumn from "../components/ResultsColumn";
import Pagination from "../components/Pagination";

const SearchResults = () => {
  const [searchParams] = useSearchParams();
  const query = searchParams.get("query");
  const [names, setNames] = useState([]);
  const [titles, setTitles] = useState([]);
  const [loading, setLoading] = useState(true);

  const [showBookmarkModal, setShowBookmarkModal] = useState(false);

  const [titlesPage, setTitlesPage] = useState(1);
  const [titlesTotalPages, setTitlesTotalPages] = useState(1);
  const [namesPage, setNamesPage] = useState(1);
  const [namesTotalPages, setNamesTotalPages] = useState(1);

  const [showMoreTitles, setShowMoreTitles] = useState(false);
  const [showMoreNames, setShowMoreNames] = useState(false);

  const handlePageChange = (type, page) => {
    if (type === "titles" && page >= 1 && page <= titlesTotalPages) {
      setTitlesPage(page);
      fetchTitlesSearch(
        query,
        page,
        setLoading,
        setTitles,
        setTitlesTotalPages
      );
    } else if (type === "names" && page >= 1 && page <= namesTotalPages) {
      setNamesPage(page);
      fetchNamesSearch(query, page, setLoading, setNames, setNamesTotalPages);
    }
  };

  const handleBookmarkClick = () => {
    setShowBookmarkModal(true);
  };

  const fetchData = async (query, namesPage, titlesPage) => {
    console.log("fetchData called with:", { query, namesPage, titlesPage });
    setLoading(true);
    try {
      await fetchNamesSearch(
        query,
        namesPage,
        setLoading,
        setNames,
        setNamesTotalPages
      );
      console.log("fetchNamesSearch completed");
      await fetchTitlesSearch(
        query,
        titlesPage,
        setLoading,
        setTitles,
        setTitlesTotalPages
      );
      console.log("fetchTitlesSearch completed");
    } catch (error) {
      console.error("Error fetching data:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (query) {
      console.log("useEffect triggered with query:", query);
      fetchData(query, namesPage, titlesPage);
    }
  }, [query, namesPage, titlesPage]);

  const getPaginationButtons = (totalPages, curPage) => {
    const buttons = [];
    const maxButtons = 5;

    if (totalPages <= maxButtons) {
      for (let i = 1; i <= totalPages; i++) {
        buttons.push(i);
      }
    } else {
      buttons.push(1);
      let startPage = Math.max(curPage - 2, 2);
      let endPage = Math.min(curPage + 2, totalPages - 1);

      if (startPage > 2) {
        buttons.push("...");
      }

      for (let i = startPage; i <= endPage; i++) {
        buttons.push(i);
      }

      if (endPage < totalPages - 1) {
        buttons.push("...");
      }

      buttons.push(totalPages);
    }

    return buttons;
  };

  if (loading) {
    return <div>Searching...</div>;
  }

  return (
    <div className="search-page">
      <FiltersColumn />
      <ResultsColumn
        query={query}
        titles={titles}
        names={names}
        showMoreTitles={showMoreTitles}
        showMoreNames={showMoreNames}
        setShowMoreTitles={setShowMoreTitles}
        setShowMoreNames={setShowMoreNames}
        handleBookmarkClick={handleBookmarkClick}
        titlesPage={titlesPage}
        titlesTotalPages={titlesTotalPages}
        namesPage={namesPage}
        namesTotalPages={namesTotalPages}
        handlePageChange={handlePageChange}
        getPaginationButtons={getPaginationButtons}
      />
      <Bookmark
        show={showBookmarkModal}
        onClose={() => setShowBookmarkModal(false)}
      />
    </div>
  );
};

export default SearchResults;
