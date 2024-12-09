const baseURL = "http://localhost:5003/api";
const userBaseURL = `${baseURL}/v3/user`;

// Function to register a user
export const registerUser = async (userData) => {
  try {
    const response = await fetch(`${userBaseURL}/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(userData),
    });

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.message || `Big Error: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error registering user:", error);
    throw error;
  }
};

// Function to login a user
export const loginUser = async (loginData) => {
  try {
    const response = await fetch(`${userBaseURL}/login`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(loginData),
      credentials: "include",
    });

    let responseData;
    if (response.headers.get("content-type")?.includes("application/json")) {
      responseData = await response.json();
    } else {
      responseData = await response.text();
    }

    if (!response.ok) {
      throw new Error(
        responseData.message || responseData || `Error: ${response.status}`
      );
    }

    return responseData;
  } catch (error) {
    console.error("Error logging in user:", error);
    throw error;
  }
};

// Function to logout a user
export const logoutUser = async () => {
  try {
    const token = localStorage.getItem("token"); // Retrieve token from localStorage

    const response = await fetch(`${userBaseURL}/logout`, {
      method: "POST",
      credentials: "include",
      headers: {
        Authorization: `Bearer ${token}`, // Include token in Authorization header
        "Content-Type": "application/json",
      },
    });

    let responseData;
    const contentType = response.headers.get("content-type");

    if (contentType && contentType.includes("application/json")) {
      responseData = await response.json();
    } else {
      responseData = await response.text();
    }

    if (!response.ok) {
      throw new Error(
        responseData.message || responseData || `Error: ${response.status}`
      );
    }

    return responseData;
  } catch (error) {
    console.error("Error logging out user:", error);
    throw error;
  }
};

export const fetchNamesSearch = async (query, pageNumber, pageSize) => {
  const params = new URLSearchParams({
    pageNumber,
    pageSize,
  });
  try {
    const response = await fetch(
      `${baseURL}/Search/name/${query}?${params.toString()}`
    );
    const nameData = await response.json();

    // Fetch images for each person in the name data
    const namesWithImages = await Promise.all(
      (nameData.items || []).map(async (person) => {
        const imageUrl = await fetchImages(person.primaryName);
        return { ...person, imageUrl };
      })
    );
    console.log(nameData);
    return {
      items: namesWithImages,
      numberPages: nameData.numberPages || 1,
    };
  } catch (error) {
    console.error("Error fetching names with images:", error);
    throw error;
  }
};

export const fetchTitlesSearch = async (
  query,
  filters,
  sortBy,
  pageNumber,
  pageSize
) => {
  const params = new URLSearchParams({
    query: query,
    sortBy,
    titleType: filters.titleType,
    genre: filters.genre,
    year: filters.year,
    pageNumber,
    pageSize,
  });
  const response = await fetch(`${baseURL}/Search/title?${params.toString()}`);
  if (!response.ok) throw new Error("Failed to fetch search results");
  return response.json();
};

//fetch all genres
export const fetchGenres = async () => {
  try {
    const response = await fetch(`${baseURL}/Data/genre`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });
    if (!response.ok) {
      throw new Error("Network response was not ok");
    }
    const genres = await response.json();
    return genres;
  } catch (error) {
    console.error("Error fetching genres:", error);
    throw error;
  }
};

const formatTitleTypes = (titleTypes) => {
  return titleTypes.map((type) => {
    let formattedType = type.titleType.replace(/([A-Z])/g, " $1").trim();
    formattedType =
      formattedType.charAt(0).toUpperCase() + formattedType.slice(1);
    formattedType = formattedType.replace(/\bTv\b/, "TV");
    return { ...type, titleType: formattedType };
  });
};

// Fetch all title types
export const fetchTitleTypes = async () => {
  try {
    const response = await fetch(`${baseURL}/Data/titletype`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });
    if (!response.ok) {
      throw new Error("Network response was not ok");
    }
    const titleTypes = await response.json();
    return formatTitleTypes(titleTypes);
  } catch (error) {
    console.error("Error fetching title types:", error);
    throw error;
  }
};

//fetch all years
export const fetchYears = async () => {
  try {
    const response = await fetch(`${baseURL}/Data/startyear`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });
    if (!response.ok) {
      throw new Error("Network response was not ok");
    }
    const years = await response.json();
    return years;
  } catch (error) {
    console.error("Error fetching years:", error);
    throw error;
  }
};

export const fetchSearchResults = async (
  query,
  pageNumber,
  setLoading,
  setNames,
  setTitles,
  setTotalPages
) => {
  setLoading(true);
  try {
    const [nameRes, titleRes] = await Promise.all([
      fetch(
        `${baseURL}/Search/name/${query}?pageNumber=${pageNumber}&pageSize=10`
      ),
      fetch(
        `${baseURL}/Search/title/${query}?pageNumber=${pageNumber}&pageSize=10`
      ),
    ]);

    const nameData = await nameRes.json();
    const titleData = await titleRes.json();

    // Fetch images for each person in the name data
    const namesWithImages = await Promise.all(
      (nameData.items || []).map(async (person) => {
        const imageUrl = await fetchImages(person.nConst);
        return { ...person, imageUrl };
      })
    );

    setNames(namesWithImages);
    setTitles(titleData.items || []);
    setTotalPages(titleData.numberPages || 1);
  } catch (error) {
    console.error("Error fetching search results:", error);
  } finally {
    setLoading(false);
  }
};

// Fetch title data by tConst
export const fetchTitleData = async (tConst) => {
  try {
    const response = await fetch(`${baseURL}/Title/${tConst}`, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch title data: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching title data:", error);
    throw error;
  }
};

// Fetch similar titles
export const fetchSimilarTitles = async (
  tConst,
  pageNumber = 1,
  pageSize = 10
) => {
  try {
    const response = await fetch(
      `${baseURL}/SimilarMovies/${tConst}?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      {
        method: "GET",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (!response.ok) {
      throw new Error(`Failed to fetch similar titles: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching similar titles:", error);
    throw error;
  }
};

// Fetch title principals
export const fetchTitlePrincipals = async (tConst) => {
  try {
    const response = await fetch(
      `${baseURL}/TitlePrincipal/${tConst}/principals`,
      {
        method: "GET",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (!response.ok) {
      throw new Error(`Failed to fetch title principals: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching title principals:", error);
    throw error;
  }
};

export const fetchNameData = async (nConst) => {
  try {
    const response = await fetch(`${baseURL}/NameBasic/${nConst}`, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch name data: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching name data:", error);
    throw error;
  }
};

export const fetchKnownForTitles = async (
  nConst,
  pageNumber = 1,
  pageSize = 10
) => {
  try {
    const response = await fetch(
      `${baseURL}/KnownForTitle/${nConst}?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      {
        method: "GET",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (!response.ok) {
      throw new Error(`Failed to fetch known for titles: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching known for titles:", error);
    throw error;
  }
};

// Function to get images of people using themovieDB API
export const fetchImages = async (nConst) => {
  try {
    // API key for the movieDB
    const apiKey = "003b3d8750e2856a2fc6e6414311d7eb";

    // Find TMDB ID for the person via name
    const response = await fetch(
      `https://api.themoviedb.org/3/find/${nConst}?external_source=imdb_id&api_key=${apiKey}`
    );
    const data = await response.json();
    if (data.person_results.length === 0) {
      return null;
    }

    const personID = data.person_results[0].id;
    // Get images of the person using the TMDB ID
    const imageResponse = await fetch(
      `https://api.themoviedb.org/3/person/${personID}/images?api_key=${apiKey}`
    );
    const imageData = await imageResponse.json();
    // profiles.filePath is where to find url
    const imgPath = imageData.profiles[0].file_path;
    const imgURL = `https://image.tmdb.org/t/p/original${imgPath}`;

    return imgURL;
  } catch (error) {
    return null;
  }
};

export const fetchBiography = async (personName) => {
  try {
    // API key for the movieDB
    const apiKey = "003b3d8750e2856a2fc6e6414311d7eb";

    // Find TMDB ID for the person via name
    const response = await fetch(
      `https://api.themoviedb.org/3/search/person?query=${personName}&api_key=${apiKey}`
    );
    const data = await response.json();

    if (data.results.length === 0) {
      return null;
    }

    const personID = data.results[0].id;

    // Get biography of the person using the TMDB ID
    const bioResponse = await fetch(
      `https://api.themoviedb.org/3/person/${personID}?api_key=${apiKey}`
    );
    const bioData = await bioResponse.json();

    // Return the biography
    return bioData.biography || "Biography not available.";
  } catch (error) {
    console.error("Error fetching biography:", error);
    return null;
  }
};

//Fetch principals by name
export const fetchPrincipalsByName = async (
  nConst,
  pageNumber = 1,
  pageSize = 10
) => {
  try {
    const response = await fetch(
      `${baseURL}/TitlePrincipal/${nConst}/principals-name?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      {
        method: "GET",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (!response.ok) {
      throw new Error(`Failed to fetch principals by name: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching principals by name:", error);
    throw error;
  }
};

//Fetch coplayers
export const fetchCoPlayers = async (nConst, pageNumber = 1, pageSize = 10) => {
  try {
    const response = await fetch(
      `${baseURL}/CoPlayers/${nConst}?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      {
        method: "GET",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (!response.ok) {
      throw new Error(`Failed to fetch coplayers: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching coplayers:", error);
    throw error;
  }
};
export const addBookmark = async (tConst, note) => {
  try {
    const token = localStorage.getItem("token");
    const response = await fetch(`${baseURL}/bookmark`, {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ tConst, note }),
    });

    if (response.status === 409) {
      console.error("Bookmark already exists");
      throw new Error("Bookmark already exists");
    }

    if (!response.ok) {
      throw new Error("Failed to add bookmark");
    }

    return await response.json();
  } catch (error) {
    console.error("Error adding bookmark:", error);
    throw error;
  }
};

export const fetchTop10Movies = async () => {
  try {
    const response = await fetch(`${baseURL}/Top10/movies`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error fetching top 10 movies:", error);
    throw error;
  }
};
export const fetchTop10TVShows = async () => {
  try {
    const response = await fetch(`${baseURL}/Top10/series`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error fetching top 10 TV shows:", error);
    throw error;
  }
};
export const fetchTop10Actors = async () => {
  try {
    const response = await fetch(`${baseURL}/Top10/actors`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error fetching top 10 actors:", error);
    throw error;
  }
};

// Function to fetch user data
export const fetchUserData = async () => {
  try {
    const token = localStorage.getItem("token"); // Retrieve token from localStorage
    const response = await fetch(`${userBaseURL}/profile`, {
      method: "GET",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`, // Include token in Authorization header
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching user data:", error);
    throw error;
  }
};

export const fetchBookmarks = async (pageNumber = 1, pageSize = 10) => {
  try {
    const token = localStorage.getItem("token");
    const response = await fetch(`${baseURL}/bookmark/user`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    if (!response.ok) {
      throw new Error("Failed to fetch bookmarks");
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error fetching bookmarks:", error);
    throw error;
  }
};

export const isTitleBookmarked = async (tConst) => {
  try {
    const bookmarksData = await fetchBookmarks();
    const bookmarks = bookmarksData.items || [];
    return bookmarks.some((bookmark) => bookmark.tConst === tConst);
  } catch (error) {
    console.error("Error checking if title is bookmarked:", error);
    return false;
  }
};
export const fetchUserRatings = async () => {
  try {
    const token = localStorage.getItem("token");
    const response = await fetch(`${baseURL}/UserRating`, {
      method: "GET",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching user ratings:", error);
    throw error;
  }
};
export const fetchUserSearchHistory = async () => {
  try {
    const token = localStorage.getItem("token");
    const response = await fetch(`${baseURL}/SearchHistory/user/`, {
      method: "GET",
      credentials: "include",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching user search history:", error);
    throw error;
  }
};
