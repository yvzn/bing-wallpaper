/*
   Copyright 2021 Yvan Razafindramanana

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

import React from "react";
import Skeleton from "react-loading-skeleton";

import getWallpapers from "./service";

class Wallpapers extends React.Component {
	constructor() {
		const self = super();
		self.state = { images: new Array(4).fill({ status: "loading" }) };
	}

	componentDidMount() {
		var self = this;
		getWallpapers().then((newImages) => {
			self.setState((previousState) => {
				const images = newImages.map((image) => ({
					...image,
					status: "loaded",
				}));
				return {
					...previousState,
					images,
				};
			});
		});
	}

	render() {
		return (
			<section>
				{this.state.images.map((image, index) => (
					<WallpaperCard image={image} key={index} />
				))}
			</section>
		);
	}
}

function WallpaperCard({ image }) {
	return (
		<article>
			<ImageTitle image={image} />
			<ImagePreview image={image} />
			<ImageCopyright image={image} />
		</article>
	);
}

function ImagePreview({ image }) {
	return (
		<p>
			{image.status === "loading" && <Skeleton />}
			{image.status === "loaded" && (
				<a href={image.url}>
					<img src={image.thumbnail} alt={image.title} />
				</a>
			)}
		</p>
	);
}

function ImageTitle({ image }) {
	return (
		<h2>
			{image.status === "loading" && <Skeleton />}
			{image.status === "loaded" && <a href={image.url}>{image.title}</a>}
		</h2>
	);
}

function ImageCopyright({ image }) {
	return (
		<footer>
			{image.status === "loading" && <Skeleton />}
			{image.status === "loaded" && image.copyright}
		</footer>
	);
}

function App() {
	return (
		<>
			<Heading />
			<Wallpapers />
			<Footnotes />
		</>
	);
}

function Heading() {
	return (
		<header>
			<h1>Wallpapers!</h1>
		</header>
	);
}

function Footnotes() {
	return (
		<footer>
			<small>
				Images from <a href="https://bing.com">Bing.com</a>. Use of
				images restricted to wallpaper only (as per Bing terms of use).
			</small>
		</footer>
	);
}

export default App;
