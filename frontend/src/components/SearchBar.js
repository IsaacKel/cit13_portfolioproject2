import { useDispatch, useSelector } from "react-redux";
import { setQuery, setCurrentPage } from "../redux/slices/searchSlice";
import { setQuery as setNamesQuery } from "../redux/slices/namesSearchSlice";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { Form, FormControl, Button } from "react-bootstrap";

const SearchBar = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { query } = useSelector((state) => state.search);

  const [searchQuery, setSearchQuery] = useState(query || "");

  const handleSearch = () => {
    dispatch(setQuery(searchQuery));
    dispatch(setNamesQuery(searchQuery));
    dispatch(setCurrentPage(1));
    if (searchQuery.trim()) {
      navigate(`/search?searchTerm=${encodeURIComponent(searchQuery)}`);
    }
  };

  const handleKeyDown = (event) => {
    if (event.key === "Enter") {
      event.preventDefault();
      handleSearch();
    }
  };

  return (
    <Form className="d-flex w-75">
      <FormControl
        type="text"
        placeholder="Search for movies, shows, or people.."
        value={searchQuery}
        onChange={(e) => setSearchQuery(e.target.value)}
        onKeyDown={handleKeyDown}
        className="mr-2 flex-grow-1"
      />
      <Button variant="outline-success" onClick={handleSearch}>
        Search
      </Button>
    </Form>
  );
};

export default SearchBar;
