﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tyrrrz.Extensions;
using YoutubeExplode.Models;

namespace YoutubeExplode.Tests
{
    // These integration tests validate that YoutubeClient correctly works with actual data.
    // This includes parsing, deciphering, downloading, etc.
    // These tests are the primary means of detecting structural changes in youtube's front end.
    // Due to the nature of youtube, some data can be very inconsistent and unpredictable.
    // Because of that, consider running tests a few times to make sure. ;)

    [TestClass]
    public class YoutubeClientIntegrationTests
    {
        private YoutubeClient _client;

        [TestInitialize]
        public void Setup()
        {
            _client = new YoutubeClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
        }

        [TestMethod]
        public async Task CheckVideoExistsAsync_Existing_Test()
        {
            string videoId = "Te_dGvF6CcE";

            bool exists = await _client.CheckVideoExistsAsync(videoId);

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task CheckVideoExistsAsync_NonExisting_Test()
        {
            string videoId = "qld9w0b-1ao";

            bool exists = await _client.CheckVideoExistsAsync(videoId);

            Assert.IsFalse(exists);
        }

        [TestMethod]
        public async Task GetVideoInfoAsync_Normal_Test()
        {
            // Most common video type

            var videoInfo = await _client.GetVideoInfoAsync("_QdPW8JrYzQ");

            Assert.IsNotNull(videoInfo);

            // Basic metadata
            Assert.AreEqual("_QdPW8JrYzQ", videoInfo.Id);
            Assert.AreEqual("This is what happens when you reply to spam email | James Veitch", videoInfo.Title);
            Assert.AreEqual("TED", videoInfo.Author);
            Assert.IsTrue(588 <= videoInfo.Length.TotalSeconds);
            Assert.IsTrue(10000000 <= videoInfo.ViewCount);
            Assert.IsTrue(4.5 <= videoInfo.AverageRating);

            // Keywords
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(6, videoInfo.Keywords.Count);

            // Watermarks
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Count);

            // Flags
            Assert.IsTrue(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            // Streams
            Assert.IsNotNull(videoInfo.Streams);
            Assert.IsTrue(20 <= videoInfo.Streams.Count);
            foreach (var streamInfo in videoInfo.Streams)
            {
                Assert.IsNotNull(streamInfo.Url);
                Assert.AreNotEqual(VideoQuality.Unknown, streamInfo.Quality);
                Assert.AreNotEqual(ContainerType.Unknown, streamInfo.Type);
                Assert.IsNotNull(streamInfo.QualityLabel);
                Assert.IsNotNull(streamInfo.FileExtension);
                Assert.IsTrue(0 < streamInfo.FileSize);
            }

            // Captions
            Assert.IsNotNull(videoInfo.ClosedCaptionTracks);
            Assert.AreEqual(42, videoInfo.ClosedCaptionTracks.Count);
            foreach (var captionTrack in videoInfo.ClosedCaptionTracks)
            {
                Assert.IsNotNull(captionTrack.Url);
                Assert.IsNotNull(captionTrack.Language);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoAsync_NonAdaptive_Test()
        {
            // Video that doesn't have embedded adaptive streams but has dash manifest

            var videoInfo = await _client.GetVideoInfoAsync("LsNPjFXIPT8");

            Assert.IsNotNull(videoInfo);

            // Basic metadata
            Assert.AreEqual("LsNPjFXIPT8", videoInfo.Id);
            Assert.AreEqual("kyoumei no true force iyasine", videoInfo.Title);
            Assert.AreEqual("Tyrrrz", videoInfo.Author);
            Assert.IsTrue(103 <= videoInfo.Length.TotalSeconds);
            Assert.IsTrue(1 <= videoInfo.ViewCount);
            Assert.IsTrue(0 <= videoInfo.AverageRating);

            // Keywords
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(0, videoInfo.Keywords.Count);

            // Watermarks
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Count);

            // Flags
            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            // Streams
            Assert.IsNotNull(videoInfo.Streams);
            Assert.IsTrue(9 <= videoInfo.Streams.Count);
            foreach (var streamInfo in videoInfo.Streams)
            {
                Assert.IsNotNull(streamInfo.Url);
                Assert.AreNotEqual(VideoQuality.Unknown, streamInfo.Quality);
                Assert.AreNotEqual(ContainerType.Unknown, streamInfo.Type);
                Assert.IsNotNull(streamInfo.QualityLabel);
                Assert.IsNotNull(streamInfo.FileExtension);
                Assert.IsTrue(0 < streamInfo.FileSize);
            }

            // Captions
            Assert.IsNotNull(videoInfo.ClosedCaptionTracks);
            Assert.AreEqual(0, videoInfo.ClosedCaptionTracks.Count);
            foreach (var captionTrack in videoInfo.ClosedCaptionTracks)
            {
                Assert.IsNotNull(captionTrack.Url);
                Assert.IsNotNull(captionTrack.Language);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoAsync_Signed_Test()
        {
            // Video that uses signature cipher

            var videoInfo = await _client.GetVideoInfoAsync("TZRvO0S-TLU");

            Assert.IsNotNull(videoInfo);

            // Basic metadata
            Assert.AreEqual("TZRvO0S-TLU", videoInfo.Id);
            Assert.AreEqual("BABYMETAL - THE ONE (OFFICIAL)", videoInfo.Title);
            Assert.AreEqual("BABYMETALofficial", videoInfo.Author);
            Assert.IsTrue(428 <= videoInfo.Length.TotalSeconds);
            Assert.IsTrue(4 <= videoInfo.AverageRating);
            Assert.IsTrue(6000000 <= videoInfo.ViewCount);

            // Keywords
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(30, videoInfo.Keywords.Count);

            // Watermarks
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Count);

            // Flags
            Assert.IsTrue(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            // Streams
            Assert.IsNotNull(videoInfo.Streams);
            Assert.IsTrue(22 <= videoInfo.Streams.Count);
            foreach (var streamInfo in videoInfo.Streams)
            {
                Assert.IsNotNull(streamInfo.Url);
                Assert.AreNotEqual(VideoQuality.Unknown, streamInfo.Quality);
                Assert.AreNotEqual(ContainerType.Unknown, streamInfo.Type);
                Assert.IsNotNull(streamInfo.QualityLabel);
                Assert.IsNotNull(streamInfo.FileExtension);
                Assert.IsTrue(0 < streamInfo.FileSize);
            }

            // Captions
            Assert.IsNotNull(videoInfo.ClosedCaptionTracks);
            Assert.AreEqual(1, videoInfo.ClosedCaptionTracks.Count);
            foreach (var captionTrack in videoInfo.ClosedCaptionTracks)
            {
                Assert.IsNotNull(captionTrack.Url);
                Assert.IsNotNull(captionTrack.Language);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoAsync_SignedRestricted_Test()
        {
            // Video that uses signature cipher and is also age-restricted

            var videoInfo = await _client.GetVideoInfoAsync("SkRSXFQerZs");

            Assert.IsNotNull(videoInfo);

            // Basic metadata
            Assert.AreEqual("SkRSXFQerZs", videoInfo.Id);
            Assert.AreEqual("HELLOVENUS 헬로비너스 - 위글위글(WiggleWiggle) M/V", videoInfo.Title);
            Assert.AreEqual("fantagio 판타지오", videoInfo.Author);
            Assert.IsTrue(203 <= videoInfo.Length.TotalSeconds);
            Assert.IsTrue(4 <= videoInfo.AverageRating);
            Assert.IsTrue(1200000 <= videoInfo.ViewCount);

            // Keywords
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(28, videoInfo.Keywords.Count);

            // Watermarks
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Count);

            // Flags
            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            // Streams
            Assert.IsNotNull(videoInfo.Streams);
            Assert.IsTrue(22 <= videoInfo.Streams.Count);
            foreach (var streamInfo in videoInfo.Streams)
            {
                Assert.IsNotNull(streamInfo.Url);
                Assert.AreNotEqual(VideoQuality.Unknown, streamInfo.Quality);
                Assert.AreNotEqual(ContainerType.Unknown, streamInfo.Type);
                Assert.IsNotNull(streamInfo.QualityLabel);
                Assert.IsNotNull(streamInfo.FileExtension);
                Assert.IsTrue(0 < streamInfo.FileSize);
            }

            // Captions
            Assert.IsNotNull(videoInfo.ClosedCaptionTracks);
            Assert.AreEqual(0, videoInfo.ClosedCaptionTracks.Count);
            foreach (var captionTrack in videoInfo.ClosedCaptionTracks)
            {
                Assert.IsNotNull(captionTrack.Url);
                Assert.IsNotNull(captionTrack.Language);
            }
        }

        [TestMethod]
        public async Task GetVideoInfoAsync_CannotEmbed_Test()
        {
            // Video that cannot be embedded outside of Youtube

            var videoInfo = await _client.GetVideoInfoAsync("_kmeFXjjGfk");

            Assert.IsNotNull(videoInfo);

            // Basic metadata
            Assert.AreEqual("_kmeFXjjGfk", videoInfo.Id);
            Assert.AreEqual("Cam'ron- Killa Kam (dirty)", videoInfo.Title);
            Assert.AreEqual("Ralph Arellano", videoInfo.Author);
            Assert.IsTrue(359 <= videoInfo.Length.TotalSeconds);
            Assert.IsTrue(4.5 <= videoInfo.AverageRating);
            Assert.IsTrue(3600000 <= videoInfo.ViewCount);

            // Keywords
            Assert.IsNotNull(videoInfo.Keywords);
            Assert.AreEqual(5, videoInfo.Keywords.Count);

            // Watermarks
            Assert.IsNotNull(videoInfo.Watermarks);
            Assert.AreEqual(2, videoInfo.Watermarks.Count);

            // Flags
            Assert.IsFalse(videoInfo.HasClosedCaptions);
            Assert.IsTrue(videoInfo.IsEmbeddingAllowed);
            Assert.IsTrue(videoInfo.IsListed);
            Assert.IsTrue(videoInfo.IsRatingAllowed);
            Assert.IsFalse(videoInfo.IsMuted);

            // Streams
            Assert.IsNotNull(videoInfo.Streams);
            Assert.IsTrue(17 <= videoInfo.Streams.Count);
            foreach (var streamInfo in videoInfo.Streams)
            {
                Assert.IsNotNull(streamInfo.Url);
                Assert.AreNotEqual(VideoQuality.Unknown, streamInfo.Quality);
                Assert.AreNotEqual(ContainerType.Unknown, streamInfo.Type);
                Assert.IsNotNull(streamInfo.QualityLabel);
                Assert.IsNotNull(streamInfo.FileExtension);
                Assert.IsTrue(0 < streamInfo.FileSize);
            }

            // Captions
            Assert.IsNotNull(videoInfo.ClosedCaptionTracks);
            Assert.AreEqual(0, videoInfo.ClosedCaptionTracks.Count);
            foreach (var captionTrack in videoInfo.ClosedCaptionTracks)
            {
                Assert.IsNotNull(captionTrack.Url);
                Assert.IsNotNull(captionTrack.Language);
            }
        }

        [TestMethod]
        public async Task GetPlaylistInfoAsync_Normal_Test()
        {
            var playlistInfo = await _client.GetPlaylistInfoAsync("PLI5YfMzCfRtZ8eV576YoY3vIYrHjyVm_e");

            Assert.IsNotNull(playlistInfo);

            // Metadata
            Assert.AreEqual("PLI5YfMzCfRtZ8eV576YoY3vIYrHjyVm_e", playlistInfo.Id);
            Assert.AreEqual("Analytics Academy - Digital Analytics Fundamentals", playlistInfo.Title);
            Assert.AreEqual("Google Analytics", playlistInfo.Author);
            Assert.AreEqual(
                "These videos are part of the Digital Analytics Fundamentals course on Analytics Academy. View the full course at http://analyticsacademy.withgoogle.com.",
                playlistInfo.Description);
            Assert.IsTrue(339000 <= playlistInfo.ViewCount);

            // Video ids
            Assert.IsNotNull(playlistInfo.VideoIds);
            CollectionAssert.AreEqual(new[]
            {
                "uPZSSdkGQhM", "JbXNS3NjIfM", "fi0w57kr_jY", "xLJt5A-NeQI", "EpDA3XaELqs", "eyltEFyZ678", "TW3gx4t4944",
                "w9H_P2wAwSE", "OyixJ7A9phg", "dzwRzUEc_tA", "vEpq3nYeZBc", "4gYioQkIqKk", "xyh8iG5mRIs", "ORrYEEH_KPc",
                "ii0T5JUO2BY", "hgycbw6Beuc", "Dz-zgq6OqTI", "I1b4GT-GuEs", "dN3gkBBffhs", "8Kg-8ZjgLAQ", "E9zfpKsw6f8",
                "eBCw9sC5D40"
            }, playlistInfo.VideoIds.ToArray());
        }

        [TestMethod]
        public async Task GetPlaylistInfoAsync_Mix_Test()
        {
            var playlistInfo = await _client.GetPlaylistInfoAsync("RDSkRSXFQerZs");

            Assert.IsNotNull(playlistInfo);

            // Metadata
            Assert.AreEqual("RDSkRSXFQerZs", playlistInfo.Id);
            // -- don't check title because it's culture specific
            Assert.IsTrue(playlistInfo.Author.IsBlank()); // mixes have no author
            Assert.IsTrue(playlistInfo.Description.IsBlank()); // and no description
            Assert.IsTrue(61000 <= playlistInfo.ViewCount);

            // Video ids (not predictable because it's a mix)
            Assert.IsNotNull(playlistInfo.VideoIds);
            Assert.AreNotEqual(0, playlistInfo.VideoIds.Count);
        }

        [TestMethod]
        public async Task GetMediaStreamAsync_Normal_Test()
        {
            var videoInfo = await _client.GetVideoInfoAsync("_QdPW8JrYzQ");

            foreach (var streamInfo in videoInfo.Streams)
            {
                using (var stream = await _client.GetMediaStreamAsync(streamInfo))
                {
                    var buffer = new byte[100];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                }
            }
        }

        [TestMethod]
        public async Task GetMediaStreamAsync_NonAdaptive_Test()
        {
            var videoInfo = await _client.GetVideoInfoAsync("LsNPjFXIPT8");

            foreach (var streamInfo in videoInfo.Streams)
            {
                using (var stream = await _client.GetMediaStreamAsync(streamInfo))
                {
                    var buffer = new byte[100];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                }
            }
        }

        [TestMethod]
        public async Task GetMediaStreamAsync_Signed_Test()
        {
            var videoInfo = await _client.GetVideoInfoAsync("9bZkp7q19f0");

            foreach (var streamInfo in videoInfo.Streams)
            {
                using (var stream = await _client.GetMediaStreamAsync(streamInfo))
                {
                    var buffer = new byte[100];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                }
            }
        }

        [TestMethod]
        public async Task GetMediaStreamAsync_SignedRestricted_Test()
        {
            var videoInfo = await _client.GetVideoInfoAsync("SkRSXFQerZs");

            foreach (var streamInfo in videoInfo.Streams)
            {
                using (var stream = await _client.GetMediaStreamAsync(streamInfo))
                {
                    var buffer = new byte[100];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                }
            }
        }

        [TestMethod]
        public async Task GetMediaStreamAsync_CannotEmbed_Test()
        {
            var videoInfo = await _client.GetVideoInfoAsync("_kmeFXjjGfk");

            foreach (var streamInfo in videoInfo.Streams)
            {
                using (var stream = await _client.GetMediaStreamAsync(streamInfo))
                {
                    var buffer = new byte[100];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                }
            }
        }

        [TestMethod]
        public async Task GetClosedCaptionTrackAsync_Test()
        {
            var videoInfo = await _client.GetVideoInfoAsync("_QdPW8JrYzQ");
            var trackInfo = videoInfo.ClosedCaptionTracks.FirstOrDefault(c => c.Language == "en");
            var track = await _client.GetClosedCaptionTrackAsync(trackInfo);
            var caption = track.GetByOffset(TimeSpan.FromSeconds(40));

            Assert.IsNotNull(track);
            Assert.IsNotNull(track.Captions);
            Assert.AreEqual("I was looking at my phone.\nI thought, I could just delete this.", caption.Text);
        }
    }
}