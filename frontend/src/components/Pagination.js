import React from "react";

const Pagination = ({
  curPage,
  totalPages,
  handlePageChange,
  getPaginationButtons,
}) => (
  <div className="pagination">
    <button
      onClick={() => handlePageChange(curPage - 1)}
      disabled={curPage === 1}
    >
      &lt;
    </button>
    {getPaginationButtons().map((page, index) => (
      <button
        key={index}
        onClick={() => page !== "..." && handlePageChange(page)}
        className={curPage === page ? "active" : ""}
        disabled={page === "..."}
      >
        {page}
      </button>
    ))}
    <button
      onClick={() => handlePageChange(curPage + 1)}
      disabled={curPage === totalPages}
    >
      &gt;
    </button>
  </div>
);

export default Pagination;
