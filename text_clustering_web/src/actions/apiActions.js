import routes from "../constants/api";

const saveToken = token => localStorage.setItem("token", token);

export const deleteToken = () => localStorage.removeItem("token");

export const getToken = () => localStorage.getItem("token");

const post = (url, payload, skipAuth) => {
    const headers = {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    };

    if (!skipAuth) {
        headers["Authorization"] = `Bearer ${getToken()}`;
    }

    return fetch(url, {
        method: 'POST',
        headers: headers,
        body: JSON.stringify(payload)
    })
};

const get = url => fetch(url, { headers: { "Authorization": "Bearer " + getToken() } });

export const cluster = async texts => {
    const response = await post(routes.DATASET_CLUSTERINGS, texts);
    return await response.json();
}

export const getDatasetClusterings = async () => {
    const response = await get(routes.DATASET_CLUSTERINGS);
    return await response.json();
}

export const getDatasetClustering = async datasetClusteringId => {
    const response = await get(routes.DATASET_CLUSTERING(datasetClusteringId));
    return await response.json();
}

export const getClusters = async datasetClusteringId => {
    const response = await get(routes.DATASET_CLUSTERS(datasetClusteringId));
    return await response.json();
}

export const login = async credentials => {
    const response = await post(routes.LOGIN, credentials, true);

    const json = await response.json();
    if (json.token) {
        saveToken(json.token);
        return true;
    }

    return false;
};