// Base URL for the API
const baseURL = "https://localhost:5003/api";

// Function to get a Title from the API
export const fetchTitle = async () => {
  try {
    const response = await fetch(`${baseURL}/Title/tt1375666`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    console.log("API Response:", data);
    return data;
  } catch (error) {
    console.error("Error fetching title:", error);
    throw error;
  }
};

// Function to post data (Just placeholder)
export const createUser = async (userData) => {
  try {
    const response = await fetch(`${baseURL}/******?`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(userData),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error creating user:", error);
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