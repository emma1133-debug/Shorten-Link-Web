import { useState, useEffect } from "react";
import { FiLink, FiScissors, FiCopy, FiClock, FiAlertTriangle } from "react-icons/fi";

export default function Index() {
  const [originalUrl, setOriginalUrl] = useState("");
  const [shortenedUrl, setShortenedUrl] = useState("");
  const [history, setHistory] = useState<{
    original: string;
    short: string;
    title: string;
    createdAt: string;
  }[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [copied, setCopied] = useState(false);
  const [errorDetails, setErrorDetails] = useState("");

  const API_BASE = "http://localhost:5050";

  useEffect(() => {
    const fetchHistory = async () => {
      try {
        const response = await fetch(`${API_BASE}/api/urls`);
        const data = await response.json();
        setHistory(data);
      } catch (err) {
        console.error("Error fetching history:", err);
        setError("Failed to fetch URL history.");
        setErrorDetails(err instanceof Error ? err.message : "Unknown error.");
      }
    };
    fetchHistory();
  }, []);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    setErrorDetails("");
    setShortenedUrl("");
    setLoading(true);

    try {
      const response = await fetch(`${API_BASE}/api/url/shorten`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ originalUrl }),
      });

      const data = await response.json();
      if (!response.ok) {
        throw new Error(data.error || "Failed to shorten the URL.");
      }

      const shortUrl = `${API_BASE}/api/url/${data.shortKey}`;
      setShortenedUrl(shortUrl);
      setHistory([
        {
          original: originalUrl,
          short: shortUrl,
          title: data.title,
          createdAt: new Date(data.createdAt).toLocaleString(),
        },
        ...history,
      ]);
    } catch (err) {
      setError("Invalid URL or the provided URL is not accessible.");
      setErrorDetails(err instanceof Error ? err.message : "Unknown error.");
    } finally {
      setLoading(false);
    }
  };

  const handleCopy = (url: string) => {
    navigator.clipboard.writeText(url);
    setCopied(true);
    setTimeout(() => setCopied(false), 1500);
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-50 px-6 py-10">
      <div className="bg-white p-8 rounded-xl shadow-md w-full max-w-lg">
        <h1 className="text-4xl font-extrabold text-center text-gray-900 mb-6 flex items-center justify-center gap-2">
          {(FiScissors as any)({ size: 32, className: "text-blue-600" })} URL Shortener
        </h1>
        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label className="block text-lg font-semibold text-gray-700">Original URL</label>
            <div className="relative mt-2">
              <input
                type="url"
                value={originalUrl}
                onChange={(e) => setOriginalUrl(e.target.value)}
                className="w-full p-4 pl-12 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none text-lg"
                placeholder="Enter a URL"
                required
              />
              {(FiLink as any)({ size: 24, className: "absolute left-4 top-4 text-gray-400" })}
            </div>
          </div>
          <button
            type="submit"
            className="w-full py-4 bg-blue-600 text-white rounded-lg font-semibold text-lg hover:bg-blue-700 transition"
            disabled={loading}
          >
            {loading ? "Shortening..." : "Shorten URL"}
          </button>
        </form>

        {error && (
          <div className="mt-4 p-4 bg-red-100 text-red-700 rounded-lg flex flex-col gap-2">
            <div className="flex items-center gap-2">
              {(FiAlertTriangle as any)({ size: 20 })} <span>{error}</span>
            </div>
            <small className="text-gray-600">{errorDetails}</small>
          </div>
        )}

        {shortenedUrl && (
          <div className="mt-6 p-5 border border-gray-300 rounded-lg bg-gray-100 flex justify-between items-center">
            <a href={shortenedUrl} target="_blank" rel="noopener noreferrer" className="text-blue-600 font-semibold truncate">
              {shortenedUrl}
            </a>
            <button
              onClick={() => handleCopy(shortenedUrl)}
              className="ml-3 p-3 bg-gray-200 rounded-full hover:bg-gray-300 transition"
            >
              {(FiCopy as any)({ size: 24, className: `text-gray-700 ${copied ? "text-green-600" : ""}` })}
            </button>
          </div>
        )}
        {copied && <p className="mt-2 text-green-600 text-lg text-center">Copied to clipboard!</p>}
      </div>

      {history.length > 0 && (
        <div className="mt-10 bg-white p-8 rounded-xl shadow-md w-full max-w-lg">
          <h2 className="text-2xl font-bold text-gray-900 flex items-center gap-2">
            {(FiClock as any)({ size: 28, className: "text-blue-600" })} URL History
          </h2>
          <div className="mt-6 max-h-72 overflow-y-auto space-y-4">
            {history.map((item, index) => (
              <div key={index} className="p-4 border border-gray-200 rounded-lg bg-gray-50">
                <span className="text-lg font-semibold text-gray-900">{item.title || "No Title"}</span>
                <a href={item.short} target="_blank" rel="noopener noreferrer" className="block text-blue-600 truncate text-lg">
                  {item.short}
                </a>
                <small className="text-gray-500">{item.createdAt}</small>
                <button
                  onClick={() => handleCopy(item.short)}
                  className="mt-2 py-2 px-4 bg-gray-200 rounded-lg hover:bg-gray-300 transition flex items-center gap-2"
                >
                  {(FiCopy as any)({ size: 18, className: "text-gray-700" })} Copy
                </button>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
