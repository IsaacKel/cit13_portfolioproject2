const baseURL = "https://localhost:5003/api";
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
        throw new Error(responseData.message || responseData || `Error: ${response.status}`);
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
    const token = localStorage.getItem('token'); // Retrieve token from localStorage

    const response = await fetch(`${userBaseURL}/logout`, {
      method: "POST",
      credentials: "include",
      headers: {
        'Authorization': `Bearer ${token}`, // Include token in Authorization header
        'Content-Type': 'application/json'
      }
    });

    let responseData;
    const contentType = response.headers.get("content-type");

    if (contentType && contentType.includes("application/json")) {
      responseData = await response.json();
    } else {
      responseData = await response.text();
    }

    if (!response.ok) {
      throw new Error(responseData.message || responseData || `Error: ${response.status}`);
    }

    return responseData;
  } catch (error) {
    console.error("Error logging out user:", error);
    throw error;
  }
};


export const fetchTitlesSearch = async (
  query,
  pageNumber,
  setLoading,
  setTitles,
  setTotalPages
) => {
  setLoading(true);
  try {
    const titleRes = await fetch(
      `${baseURL}/Search/title/${query}?pageNumber=${pageNumber}&pageSize=10`
    );

    const titleData = await titleRes.json();

    setTitles(titleData.items || []);
    setTotalPages(titleData.numberPages || 1);
  } catch (error) {
    console.error("Error fetching title search results:", error);
  } finally {
    setLoading(false);
  }
};

//Fetch titles filtered by numvotes
export const fetchTitlesByNumVotes = async (
  query,
  pageNumber,
  setLoading,
  setTitles,
  setTotalPages,
  titleType = null,
  genre = null,
  year = -1
) => {
  setLoading(true);
  try {
    const titleRes = await fetch(
      `${baseURL}/Search/title/numvotes?searchTerm=${encodeURIComponent(
        query
      )}&searchTitleType=${titleType}&searchGenre=${genre}&searchYear=${year}&pageNumber=${pageNumber}&pageSize=10`
    );
    const titleData = await titleRes.json();
    setTitles(titleData.items || []);
    setTotalPages(titleData.numberPages || 1);
  } catch (error) {
    console.error("Error fetching title search results:", error);
  } finally {
    setLoading(false);
  }
};

//Fetch titles filtered by rating
export const fetchTitlesByRating = async (
  query,
  pageNumber,
  setLoading,
  setTitles,
  setTotalPages,
  titleType = null,
  genre = null,
  year = -1
) => {
  setLoading(true);
  try {
    const titleRes = await fetch(
      `${baseURL}/Search/title/rating?searchTerm=${encodeURIComponent(
        query
      )}&searchTitleType=${titleType}&searchGenre=${genre}&searchYear=${year}&pageNumber=${pageNumber}&pageSize=10`
    );
    const titleData = await titleRes.json();
    setTitles(titleData.items || []);
    setTotalPages(titleData.numberPages || 1);
  } catch (error) {
    console.error("Error fetching title search results:", error);
  } finally {
    setLoading(false);
  }
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

export const fetchNamesSearch = async (
  query,
  pageNumber,
  setLoading,
  setNames,
  setTotalPages
) => {
  setLoading(true);
  try {
    const nameRes = await fetch(
      `${baseURL}/Search/name/${query}?pageNumber=${pageNumber}&pageSize=10`
    );

    const nameData = await nameRes.json();

    // Fetch images for each person in the name data
    const namesWithImages = await Promise.all(
      (nameData.items || []).map(async (person) => {
        const nConst = person.nConst.split("/").pop();
        const imageUrl = await fetchImages(nConst);
        return { ...person, imageUrl };
      })
    );

    setNames(namesWithImages);
    setTotalPages(nameData.numberPages || 1);
  } catch (error) {
    console.error("Error fetching name search results:", error);
  } finally {
    setLoading(false);
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

//Fetch principals by name
export const fetchPrincipalsByName = async (nConst) => {
  try {
    const response = await fetch(
      `${baseURL}/TitlePrincipal/${nConst}/principals-name`,
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
export const fetchCoPlayers = async (nConst) => {
  try {
    const response = await fetch(`${baseURL}/CoPlayers/${nConst}`, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch coplayers: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching coplayers:", error);
    throw error;
  }
};

// Function to add a bookmark
export const addBookmark = async (userId, tConst, note) => {
  try {
    const response = await fetch(`${baseURL}/bookmarks`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ userId, tConst, note }),
    });

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
    const token = localStorage.getItem('token'); // Retrieve token from localStorage
    const response = await fetch(`${userBaseURL}/profile`, {
        method: "GET",
        credentials: "include",
      headers: {
        "Content-Type": "application/json",
        'Authorization': `Bearer ${token}`, // Include token in Authorization header
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
}
export const fetchUserBookmarks = async (userId) => {
  try {
    const response = await fetch(`${baseURL}/Bookmark/user/${userId}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching user bookmarks:", error);
    throw error;
  }
}
export const fetchUserRatings = async (userId) => {
  try {
    const response = await fetch(`${baseURL}/UserRating/${userId}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
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
}
export const fetchUserSearchHistory = async (userId) => {
  try {
    const response = await fetch(`${baseURL}/SearchHistory/user/${userId}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
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
}
