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
        const response = await fetch(`${userBaseURL}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(loginData),
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || `Error: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error("Error logging in user:", error);
        throw error;
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
export const fetchSimilarTitles = async (tConst, pageNumber = 1, pageSize = 10) => {
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
        const response = await fetch(`${baseURL}/TitlePrincipal/${tConst}/principals`, {
            method: "GET",
            headers: { "Content-Type": "application/json" },
        });

        if (!response.ok) {
            throw new Error(`Failed to fetch title principals: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error("Error fetching title principals:", error);
        throw error;
    }
};

// Function to get images of people using themovieDB API
export const fetchImages = async (personName) => {
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
}
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
}
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
}