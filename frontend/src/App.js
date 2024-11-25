import "bootstrap/dist/css/bootstrap.min.css";
import "./index.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import NavBar from "./components/NavBar";
import Footer from "./components/Footer";

import Login from "./pages/Login";
import HomePage from "./pages/HomePage";
import IndividualTitle from "./pages/IndividualTitle";
import SearchResults from "./pages/SearchResults";

const App = () => {
  return (
    <div id="app-container">
      <Router>
        <NavBar />
        <div className="main-content">
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/search" element={<SearchResults />} />
            <Route path="/login" element={<Login />} />
            <Route path="/title/:tConst" element={<IndividualTitle />} />
          </Routes>
        </div>
        <Footer />
      </Router>
    </div>
  );
};

export default App;
