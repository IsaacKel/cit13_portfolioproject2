import React from "react";
import SearchItem from "./SearchItem";
import Pagination from "./Pagination";

const ResultsColumn = ({
  query,
  titles,
  names,
  showMoreTitles,
  showMoreNames,
  setShowMoreTitles,
  setShowMoreNames,
  handleBookmarkClick,
  titlesPage,
  titlesTotalPages,
  namesPage,
  namesTotalPages,
  handlePageChange,
  getPaginationButtons,
}) => {
  const displayedTitles = showMoreTitles ? titles : titles.slice(0, 3);
  const displayedPeople = showMoreNames ? names : names.slice(0, 3);

  return (
    <div className="results-column">
      <h2>Search Results for "{query}"</h2>
      <h3>Titles</h3>
      {titles.length === 0 && <p>No results found</p>}
      <div className="search-results-container">
        {displayedTitles.map((title, index) => (
          <SearchItem
            key={index}
            item={title}
            type="title"
            handleBookmarkClick={handleBookmarkClick}
          />
        ))}
      </div>
      {titles.length > 3 && !showMoreTitles && (
        <p onClick={() => setShowMoreTitles(true)} className="see-more-text">
          See more...
        </p>
      )}
      {showMoreTitles && (
        <>
          <p onClick={() => setShowMoreTitles(false)} className="see-more-text">
            See less...
          </p>
          <Pagination
            curPage={titlesPage}
            totalPages={titlesTotalPages}
            handlePageChange={(page) => handlePageChange("titles", page)}
            getPaginationButtons={() =>
              getPaginationButtons(titlesTotalPages, titlesPage)
            }
          />
        </>
      )}
      <h3>People</h3>
      <div className="search-results-container">
        {names.length === 0 && <p>No people found</p>}
        {displayedPeople.map((name, index) => (
          <SearchItem
            key={index}
            item={name}
            type="person"
            handleBookmarkClick={handleBookmarkClick}
          />
        ))}
        {names.length > 3 && !showMoreNames && (
          <p onClick={() => setShowMoreNames(true)} className="see-more-text">
            See more...
          </p>
        )}
        {showMoreNames && (
          <>
            <p
              onClick={() => setShowMoreNames(false)}
              className="see-more-text"
            >
              See less..
            </p>
            <Pagination
              curPage={namesPage}
              totalPages={namesTotalPages}
              handlePageChange={(page) => handlePageChange("names", page)}
              getPaginationButtons={() =>
                getPaginationButtons(namesTotalPages, namesPage)
              }
            />
          </>
        )}
      </div>
    </div>
  );
};

export default ResultsColumn;
